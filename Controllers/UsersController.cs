using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RestaurantAPI.DTO;
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

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
        public void Delete(int id)
        {
        }
    }
}
