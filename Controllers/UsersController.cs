using AutoMapper;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using test_crud.Entities;
using test_crud.Models;
using test_crud.Services;
using test_crud.Validators;

namespace test_crud.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class UsersController: ControllerBase
    {
        public IConfiguration config { get; set; }
        public IUsersRepository userRepo { get; set; }

        public IWebHostEnvironment env { get; set; }

        public IMapper mapper { get; set; }
        public UsersController(IUsersRepository userRepo,IMapper mapper, IWebHostEnvironment env, IConfiguration config)
        {
            this.userRepo=userRepo;
            this.mapper = mapper;
            this.env = env;
            this.config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDetailsWithTokenDto>> LoginUser(LoginDto loginDto)
        {
            try
            {
                var user = await userRepo.GetValueByExpression(v => v.Username == loginDto.Username);

                if (user == null || !Argon2.Verify(user.Password, loginDto.Password))
                {
                    return Unauthorized(new
                    {
                        errorMessage = "Invalid Login"
                    });
                }

                var result = mapper.Map<UserDetailsWithTokenDto>(user);

                var accessToken = userRepo.GenerateJwtToken(user.Id, false);
                var refreshToken = userRepo.GenerateJwtToken(user.Id, true);
                result.AccessToken = accessToken;

                Response.Cookies.Append("rt", refreshToken, new CookieOptions()
                {
                    MaxAge = TimeSpan.FromDays(7),
                    HttpOnly = true
                });

                return Ok(result);
            } catch(Exception ex)
            {
                return BadRequest(new
                {
                    errorMessage = "error" + ex.Message
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDetailsWithTokenDto>> RegisterUser(RegisterDto registerDto)
        {
            try
            {
                var validator = new RegisterValidator();

                var result = validator.Validate(registerDto);

                if(!result.IsValid)
                {
                    return BadRequest(result.Errors);
                }


                var mappedUser = mapper.Map<Users>(registerDto);

                var salt = RandomNumberGenerator.GetBytes(128);
                var config = new Argon2Config()
                {
                    Salt = salt,
                    Password = Encoding.UTF8.GetBytes(mappedUser.Password)
                };

                mappedUser.Password = Argon2.Hash(config);

                await userRepo.Create(mappedUser);

                var mapped = mapper.Map<UserDetailsWithTokenDto>(mappedUser);

                var accessToken = userRepo.GenerateJwtToken(mapped.Id, false);
                var refreshToken = userRepo.GenerateJwtToken(mapped.Id, true);
                mapped.AccessToken = accessToken;


                Response.Cookies.Append("rt", refreshToken, new CookieOptions()
                {
                    MaxAge = TimeSpan.FromDays(7),
                    HttpOnly = true,
                    Secure = !this.env.IsDevelopment() && true
                });

                return Ok(mapped);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("refresh")]
        public async Task<ActionResult<UserDetailsWithTokenDto>> RefreshToken ()
        {
            try
            {
                var oldToken = Request.Cookies["rt"];

                if (oldToken == null)
                {
                    return Unauthorized(new
                    {
                        errorMesasge = "No Token"
                    });
                }

                var decoded = new JwtSecurityTokenHandler().ValidateToken(oldToken, new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Authentication:Issuer"],
                    ValidAudience = config["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Authentication:SecretForKey"]!))
                }, out SecurityToken securityToken);


                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Unauthorized(new
                    {
                        errorMessage = "Invalid Token"
                    });
                }

                if (decoded.Identity == null)
                {
                    return Unauthorized();
                }

                var userId = decoded.Identity.Name;

                if (userId == null)
                {
                    return Unauthorized();
                }

                var user = await userRepo.GetValueByExpression(v => v.Id == new Guid(userId));

                if (user == null)
                {
                    return Unauthorized();
                }

                var mappedUser = mapper.Map<UserDetailsWithTokenDto>(user);


                var accessToken = userRepo.GenerateJwtToken(mappedUser.Id, false);
                var refreshToken = userRepo.GenerateJwtToken(mappedUser.Id, true);
                mappedUser.AccessToken = accessToken;


                Response.Cookies.Append("rt", refreshToken, new CookieOptions()
                {
                    MaxAge = TimeSpan.FromDays(7),
                    HttpOnly = true,
                    Secure = !this.env.IsDevelopment() && true
                });

                return Ok(mappedUser);
            } catch(Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("logout")]
        public ActionResult LogoutUser()
        {
            Response.Cookies.Append("rt", "", new CookieOptions()
            {
                MaxAge = TimeSpan.Zero,
                HttpOnly = true,
                Secure = !this.env.IsDevelopment() && true
            });

            return Ok(new
            {
                errorMessage = "User logged out"
            });
        }
    }
}
