using Microsoft.AspNetCore.Authorization;
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
using System.Net.Http;
using System.Net;

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
                Response.Cookies.Append("Refresh-Token", GenerateRefresh(newUser));
                return Ok(AuthSuccess(newUser));
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
                return BadRequest(AuthFail("User does not exist"));
            }

            var isCorrect = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isCorrect)
            {
                return BadRequest(AuthFail("Invalid credentials"));
            }
            else
            {
                Response.Cookies.Append("Refresh-Token", GenerateRefresh(user));
                return Ok(AuthSuccess(user));
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> refreshToken()
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            string refreshToken = Request.Cookies["Refresh-Token"];
            if (refreshToken == null)
            {
                return BadRequest(AuthFail("No Token"));
            }
            var result = await VerifyRefreshGenerateToken(refreshToken);
            if (result.Result == false)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        private async Task<AuthResult> VerifyRefreshGenerateToken(string refreshToken)
        {
            try
            {
                var storedToken =  _context.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
                if(storedToken == null)
                {
                    return AuthFail("Invalid Token");
                }
                if (storedToken.Revoked)
                {
                    return AuthFail("Invalid Token");
                }
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return AuthFail("Expired Token");
                }

                var user = await _userManager.FindByIdAsync(storedToken.UserId);

                if (user == null)
                {
                    return AuthFail("User does not exist");
                }

                return new AuthResult()
                {
                    Result = true,
                    Token = GenerateJwt(user)
                };
            }
            catch (Exception e)
            {
                return AuthFail("Server Error");
            }
        }
        private string GenerateJwt(IdentityUser user)
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
            return tokenHandler.WriteToken(token);
        }
        private string GenerateRefresh(IdentityUser user)
        {
            var refreshToken = new RefreshToken()
            {
                Token = RandomNumberGenerator.GetHexString(30, true),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                Revoked = false,
                UserId = user.Id
            };
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            return refreshToken.Token;
        }
        private AuthResult AuthSuccess(IdentityUser user)
        {
            return new AuthResult()
            {
                Result = true,
                Token = GenerateJwt(user)
            };
        }
        private AuthResult AuthFail(string error)
        {
            return new AuthResult()
            {
                Result = false,
                Errors = new List<string>() { error }
            };
        }
    }
}
