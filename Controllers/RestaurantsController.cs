using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Org.BouncyCastle.Crypto.Generators;
using RestaurantAPI.DTO;
using RestaurantAPI.ENUM;
using RestaurantAPI.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        IConfiguration _configuration;

        public RestaurantsController(RestaurantContext context, EmailService emailService, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _emailService = emailService;
            _mapper = mapper;
            _configuration = configuration;
        }

        // GET: api/Restaurants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurant()
        {
            return await _context.Restaurant.ToListAsync();
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetRestaurantStats()
        {
            var total = await _context.Restaurant.Where(r => r.Status != (int)RestaurantStatus.Rejected).CountAsync();
            var active = await _context.Restaurant.Where(r => r.Status == (int)RestaurantStatus.Active).CountAsync();
            var pending = await _context.Restaurant.Where(r => r.Status == (int)RestaurantStatus.Pending).CountAsync();

            return Ok(new { success = true, total, active, pending });
        }

        [HttpGet("stats/{id}")]
        public async Task<ActionResult> GetParticularRestaurantStats(int id)
        {
            var totalorders = await _context.Order
                .Where(o => o.RestaurantId == id).CountAsync();
            var totalusers = await _context.Users.Where(u => u.RestaurantId == id && u.RoleId == 3).CountAsync();

            return Ok(new { success = true,totalorders,totalusers });
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurant.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // PUT: api/Restaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestaurant(int id, RestaurantRequest request)
        {
            if (request.RestaurantId != request.RestaurantId)
            {
                return BadRequest();
            }

            //_context.Entry(restaurant).State = EntityState.Modified;

            var newRes = await _context.Restaurant.FindAsync(request.RestaurantId);

            if (newRes == null)
            {
                return NotFound();
            }

            try
            {
                _mapper.Map(request, newRes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Updated Successfully");
        }

        // POST: api/Restaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Restaurant>> PostRestaurant(RestaurantRequest request)
        {
            try
            {
                var newRes = _mapper.Map<RestaurantRequest, Restaurant>(request);
                _context.Restaurant.Add(newRes);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetRestaurant", new { id = newRes.RestaurantId }, newRes);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var users = _context.Users.Where(u => u.RestaurantId == id);
            _context.Users.RemoveRange(users);
            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            _context.Restaurant.Remove(restaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurant.Any(e => e.RestaurantId == id);
        }

        public class RestaurantStatusRequest
        {
            public int Status { get; set; }
        }

        [HttpPost("approve_reject/{id}")]
        public async Task<IActionResult> AcceptOrRejectRequest(int id, [FromBody] RestaurantStatusRequest request)
        {
            Console.WriteLine(request.Status);
            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            // Convert integer to enum
            if (Enum.IsDefined(typeof(RestaurantStatus), request.Status))
            {
                restaurant.Status = (int)(RestaurantStatus)request.Status;
                await _context.SaveChangesAsync();
                string subject = "";
                string htmlBody = "";
                string recipientEmail = restaurant.Email;
                Console.WriteLine(restaurant.Email);

                switch (restaurant.Status)
                {
                    case (int)RestaurantStatus.Active:
                        string randomPassword = PasswordGenerator.generateRandomPassword();

                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(randomPassword);

                        var newUser = new User
                        {
                            Email = restaurant.Email,
                            Password = hashedPassword,
                            RestaurantId = restaurant.RestaurantId,
                            RoleId = 2,
                            IsFirstLogin = true,
                            Status = 1,
                            MobileNo = restaurant.MobileNo
                        };

                        _context.Users.Add(newUser);
                        await _context.SaveChangesAsync();

                        subject = "Your Restaurant Admin Account is Ready";

                        htmlBody = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
                                <h2 style='color: #333; text-align: center;'>Welcome to Restaurant Management</h2>
                                <p style='font-size: 16px; color: #555;'>Hello,</p>
                                <p style='font-size: 16px; color: #555;'>
                                    Your admin account has been created. Below are your login credentials:
                                </p>
                                <div style='background-color: #fff; padding: 15px; border-radius: 5px; border: 1px solid #ccc;'>
                                    <p style='font-size: 16px;'><strong>Email:</strong> {restaurant.Email}</p>
                                    <p style='font-size: 16px;'><strong>Password:</strong> {randomPassword}</p>
                                </div>
                                <p style='font-size: 16px; color: #555;'>
                                    Please log in and change your password immediately for security reasons.
                                </p>
                                <p style='text-align: center; margin-top: 20px;'>
                                    <a href='https://yourwebsite.com/login' style='background-color: #007bff; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-size: 16px;'>Login Now</a>
                                </p>
                                <p style='font-size: 16px; color: #555;'><strong>Best regards,</strong><br>Restaurant Management Team</p>
                            </div>";
                        Console.WriteLine("New Customer Admin Is Created");
                        break;

                    case (int)RestaurantStatus.Rejected:
                        subject = "Your Restaurant Registration was Rejected";
                        htmlBody = $@"
                    <h2>Dear {restaurant.Name},</h2>
                    <p>Unfortunately, your restaurant registration has been rejected.</p>
                    <p>Please contact support for more details.</p>
                    <p>Best regards,</p>
                    <p>Restaurant Management Team</p>";
                        break;
                }

                if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(htmlBody))
                {
                    await _emailService.SendEmailAsync(recipientEmail, subject, htmlBody);
                }
                return NoContent();
            }
            else
            {
                return BadRequest("Invalid status value.");
            }
        }

        [HttpGet("accepted")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> ApprovedRestaurants()
        {
            return await _context.Restaurant
                .Where(r => r.Status == (int)RestaurantStatus.Active || r.Status == (int)RestaurantStatus.Inactive)
                .ToListAsync();
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> PendingRestaurants()
        {
            return await _context.Restaurant
                .Where(r => r.Status == (int)RestaurantStatus.Pending)
                .ToListAsync();
        }
    }
}
