using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RestaurantAPI.DTO;
using RestaurantAPI.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly EmailService _emailService;

        public UsersController(RestaurantContext context, EmailService emailSservice)
        {
            _context = context;
            _emailService = emailSservice;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await _context.Users.ToListAsync();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("role/{rid}")]
        public async Task<ActionResult<User>> GetUsersByRole(int rid)
        {
            var users = await _context.Users.Where(u => u.RoleId == rid).ToListAsync();

            return Ok(users);
        }

        [HttpGet("customeradmin/stats")]
        public async Task<ActionResult> GetCustomerAdminStats()
        {
            var total = await _context.Users.Where(u => u.RoleId == 2).CountAsync();
            var active = await _context.Users.Where(u => u.RoleId == 2 && u.Status == 1).CountAsync();
            var inactive = await _context.Users.Where(u => u.RoleId == 2 && u.Status == 0).CountAsync();

            return Ok(new { success = true, total, active, inactive });
        }

        [HttpGet("type/{rid}")]
        public async Task<ActionResult<User>> GetUsersByType(string type)
        {
            var users = await _context.Users.Where(u => u.UserType == type).ToListAsync();

            return Ok(users);
        }

        // Get all users of a specific restaurant
        [HttpGet("RestaurantUsers/{rid}")]
        public async Task<ActionResult> GetUsersOfRestaurant(int rid)
        {
            var users = await _context.Users.Where(us => us.RestaurantId == rid).ToListAsync();
            return Ok(new
            {
                users
            });
        }

        [HttpGet("restaurant/users/{rid}/{type}")]
        public async Task<ActionResult> GetUsersByTypeAndRestaurant(int rid, string type)
        {
            var users = await _context.Users.Where(us => us.RestaurantId == rid && us.UserType == type).ToListAsync();
            return Ok(new
            {
                users
            });
        }

        [HttpGet("Restaurant/{rid}")]
        public async Task<ActionResult> GetCustomerUserOfRestaurant(int rid)
        {
            var users = await _context.Users.Where(us => us.RestaurantId == rid && us.UserType != null).ToListAsync();

            return Ok(new
            {
                users
            });
        }

        // POST api/<UsersController>
        [HttpPost("CustomerAdmin/CreateUser")]
        public async Task<ActionResult> Post([FromBody] CustomerUserRequest request)
        {
            string randomPassword = PasswordGenerator.generateRandomPassword();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            User u = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                FullName = request.FullName,
                UserType = request.UserType,
                Status = 1,
                RoleId = 3,
                RestaurantId = request.RestaurantId,
                MobileNo = request.MobileNo
            };

            _context.Users.Add(u);
            await _context.SaveChangesAsync();

            string recipientEmail = u.Email;

            string subject = "Your Customer User Account is Ready";

            string htmlBody = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
                            <h2 style='color: #333; text-align: center;'>Welcome to Restaurant Management</h2>
                            <p style='font-size: 16px; color: #555;'>Hello,</p>
                            <p style='font-size: 16px; color: #555;'>
                                Your customer user account has been created. Below are your login credentials:
                            </p>
                            <div style='background-color: #fff; padding: 15px; border-radius: 5px; border: 1px solid #ccc;'>
                                <p style='font-size: 16px;'><strong>Email:</strong> {u.Email}</p>
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

            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(htmlBody))
            {
                await _emailService.SendEmailAsync(recipientEmail, subject, htmlBody);
            }


            return Ok(new
            {
                Success = true,
                user = u
            });
        }

        // PUT api/<UsersController>/5
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changeP)
        {
            User u = await _context.Users.FindAsync(changeP.UserId);

            if (u == null)
                return NotFound(new { message = "User not found" });

            var verify = BCrypt.Net.BCrypt.Verify(changeP.OldPassword, u.Password);

            if (verify == false)
                return BadRequest(new { message = "Old password is incorrect" });

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(changeP.NewPassword);

            u.Password = hashPassword;
            u.IsFirstLogin = false;

            await _context.SaveChangesAsync();

            string subject = "Your Password Has Been Changed Successfully";

            string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
                    <h2 style='color: #333; text-align: center;'>Password Changed Successfully</h2>
                    <p style='font-size: 16px; color: #555;'>Hello,</p>
                    <p style='font-size: 16px; color: #555;'>
                        This is to confirm that your account password has been changed successfully.
                    </p>
                    <p style='font-size: 16px; color: #555;'>
                        If you made this change, no further action is required.
                    </p>
                    <p style='font-size: 16px; color: #555;'>
                        If you did not request or authorize this password change, please reset your password immediately or contact our support team.
                    </p>
                    <p style='text-align: center; margin-top: 20px;'>
                        <a href='https://yourwebsite.com/login' style='background-color: #28a745; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-size: 16px;'>Login to Your Account</a>
                    </p>
                    <p style='font-size: 16px; color: #555; margin-top: 30px;'><strong>Stay safe,</strong><br>Restaurant Management Team</p>
                </div>";

            await _emailService.SendEmailAsync(u.Email, subject, htmlBody);


            return Ok(new { message = "Password changed successfully" });
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Optional: You can use a soft delete by updating a status flag instead
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Send email
            string subject = "Account Deleted - Restaurant Management System";
            string body = $@"
            Dear {user.FullName},

            Your account associated with this email ({user.Email}) has been deleted from our restaurant management system.

            If you believe this is a mistake or have any questions, please contact our support team.

            Best regards,
            The Restaurant Team";

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                // Optional: log email failure, but don't block deletion
                Console.WriteLine("Email sending failed: " + ex.Message);
            }

            return Ok(new { message = "User deleted and email sent." });
        }

    }
}
