using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RestaurantAPI.Models;
using Microsoft.Extensions.Options;
using RestaurantAPI.DTO;

namespace RestaurantAPI.Services
{
    public class AuthService
    {
        private readonly RestaurantContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(RestaurantContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (_jwtSettings == null || string.IsNullOrEmpty(_jwtSettings.Key))
            {
                throw new Exception("JWT settings are not configured properly.");
            }
        }

        public async Task<AuthResponse> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;  // Invalid login

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            Console.WriteLine(user.Email);
            Console.WriteLine(user.IsFirstLogin);

            var claims = new List<Claim>
            {   
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("IsFirstLogin",user.IsFirstLogin.ToString())
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return new AuthResponse
            {
                Token = tokenString,
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.RoleName,
                IsFirstLogin = user.IsFirstLogin
            };
        }
    }
}
