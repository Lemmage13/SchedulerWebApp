using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerWebApp.Server.Configurations;
using SchedulerWebApp.Server.Data;
using SchedulerWebApp.Server.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchedulerWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly PlannerContext _context;
        private readonly TokenValidationParameters _validationParams;

        public AuthController(UserManager<IdentityUser> uM, IOptions<JwtConfig> jC, PlannerContext context, TokenValidationParameters validationParams)
        {
            _userManager = uM;
            _jwtConfig = jC.Value;
            _context = context;
            _validationParams = validationParams;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationRequestDTO registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (user != null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email already in use"
                    }
                });
            }

            var newUser = new IdentityUser()
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Name
            };
            IdentityResult isCreated = await _userManager.CreateAsync(newUser, registerRequest.Password);

            if (isCreated.Succeeded)
            {
                return Ok(GenerateAuthTokens(newUser));
            }
            else
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Server Error",
                        isCreated.ToString()
                    }
                });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]UserLoginRequestDTO loginRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email not registered"
                    }
                });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isCorrect)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalid Credentials"
                    }
                });
            }
            else
            {
                return Ok(GenerateAuthTokens(user));
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> refreshToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var result = await VerifyRefreshGenerateToken(request);
            if (result == null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalid token"
                    }
                });
            }
            if (result.Result == false)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        private async Task<AuthResult> VerifyRefreshGenerateToken(TokenRequest request)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _validationParams.ValidateLifetime = false; //TESTING ONLY

                var verToken = tokenHandler.ValidateToken(request.Token, _validationParams, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    bool result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256);
                    if (!result) 
                    { 
                        return new AuthResult()
                        {
                            Result = false,
                            Errors = new List<string>()
                            {
                                "Invalid token"
                            }
                        }; 
                    }
                }

                DateTime expDate = StringTimestampToDateTime(verToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
                if (expDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                            {
                                "Unexpired token"
                            }
                    };
                }
                var storedToken =  _context.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);
                if(storedToken == null)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                            {
                                "Invalid token"
                            }
                    };
                }

                if (storedToken.Used || storedToken.Revoked)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                            {
                                "Invalid token"
                            }
                    };
                }
                var jti = verToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
                if (jti != storedToken.JwtId)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                            {
                                "Invalid token"
                            }
                    };
                }
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                            {
                                "Expired token"
                            }
                    };
                }

                storedToken.Used = true;
                _context.RefreshTokens.Update(storedToken);
                _context.SaveChanges();

                var user = await _userManager.FindByIdAsync(storedToken.UserId);

                return GenerateAuthTokens(user);
            }
            catch (Exception e)
            {
                return new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                            {
                                "Server Error"
                            }
                };
            }
        }
        private DateTime StringTimestampToDateTime(string date)
        {
            long longDate = long.Parse(date);
            DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return startDate.AddSeconds(longDate).ToUniversalTime();
        }
        private AuthResult GenerateAuthTokens(IdentityUser user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            if (_jwtConfig == null)
            {
                throw new ArgumentNullException(nameof(_jwtConfig));
            }

            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = RandomNumberGenerator.GetHexString(30, true),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                Revoked = false,
                Used = false,
                UserId = user.Id
            };
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            return new AuthResult()
            {
                Result = true,
                Token = jwt,
                RefreshToken = refreshToken.Token,
            };
        }
    }
}
