using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Email, email),
            //    new Claim(ClaimTypes.Role, role),  // Role-based claim
            //    new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // User ID claim
            //    new Claim("IsFirstLogin", isFirstLogin.ToString()) // Custom claim for first login
            //};

            var claims = new List<Claim>
            {
                new Claim("userId", userId.ToString())
            };


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            //Console.WriteLine("Generated JWT: " + token);
            return tokenHandler.WriteToken(token);
        }

        //// ✅ Fix: Remove "NotImplementedException" and implement this properly
        //public string GenerateToken(string email, Role role, int userId, bool isFirstLogin)
        //{
        //    return GenerateToken(email, role.RoleName.ToString(), userId, isFirstLogin);  // Call the correct method
        //}
    }
}
