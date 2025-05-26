using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly JwtService _jwtService;

        public AuthController(RestaurantContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest1 request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            Console.WriteLine("Stored Hashed Password: " + user.Password);
            Console.WriteLine("Entered Password: " + request.Password);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            if (user.Role == null)
            {
                return BadRequest("User role is not assigned.");
            }

            Console.WriteLine("User Role: " + user.Role.RoleName);
            Console.WriteLine("User is first login: " + user.IsFirstLogin);

            var token = _jwtService.GenerateToken(user.UserId);
            Console.WriteLine("Generated JWT: " + token);

            return Ok(new
            {
                Token = token,
                Roles = user.Role.RoleName

            });
        }

        [HttpGet("decode-token")]
        public async Task<IActionResult> GetUserFromToken()
        {
            try
            {
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Token Not Found or Invalid" });
                }

                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var user = new
                {
                    UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                };

                User u = await _context.Users
                    .Include(u => u.Role)
                    .FirstAsync(u => u.UserId == int.Parse(user.UserId));

                //foreach (var claim in jwtToken.Claims)
                //{
                //    Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                //}

                var res = new
                {
                    Email = u.Email,
                    UserId = u.UserId,
                    IsFirstLogin = u.IsFirstLogin,
                    Role = u.Role.RoleName,
                    RestaurantId = u.RestaurantId,
                    UserType = u?.UserType
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [Authorize]
        [HttpGet("get-current-user")]
        public IActionResult GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                var userClaims = identity.Claims;

                return Ok(new
                {
                    Email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    UserId = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                    IsFirstLogin = userClaims.FirstOrDefault(x => x.Type == "IsFirstLogin")?.Value
                });
            }

            return Unauthorized();
        }

    }
}
