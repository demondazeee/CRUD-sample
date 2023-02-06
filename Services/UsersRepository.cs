using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test_crud.DBContext;
using test_crud.Entities;
using static System.Net.WebRequestMethods;

namespace test_crud.Services
{
    public class UsersRepository : RepositoryBase<Users>, IUsersRepository
    {
        public IHttpContextAccessor httpContext { get; set; }
        public IConfiguration config { get; set; }
        public UsersRepository(DB context, IConfiguration config, IHttpContextAccessor httpContext) : base(context)
        {
            this.config = config;
            this.httpContext = httpContext;
        }

        public string GenerateJwtToken(Guid Id, bool isRefreshToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Authentication:SecretForKey"]!));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, Id.ToString())
            };

            var securityToken = new JwtSecurityToken(
                    config["Authentication:Issuer"],
                    config["Authentication:Audience"],
                    claims,
                    DateTime.UtcNow,
                    isRefreshToken ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(15),
                    signingCredentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }

        public string GetAuthenticatedUserId()
        {
            var userId = string.Empty;
            if (httpContext.HttpContext != null)
            {
                userId = httpContext.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }

            if(userId == null)
            {
                throw new ArgumentNullException(userId, "User Not found");
            }

            return userId;
        }
    }
}
