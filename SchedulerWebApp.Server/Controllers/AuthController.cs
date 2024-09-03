using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerWebApp.Server.Configurations;
using SchedulerWebApp.Server.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchedulerWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthController(UserManager<IdentityUser> uM, IOptions<JwtConfig> jC)
        {
            _userManager = uM;
            _jwtConfig = jC.Value;
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
                var token = GenerateJwtToken(newUser);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });
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
                var token = GenerateJwtToken(user);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });
            }
        }



        private string GenerateJwtToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

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
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
