using MailKit.Net.Smtp;
using MimeKit;

namespace RestaurantAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Restaurant Management", _configuration["EmailSettings:From"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _configuration["EmailSettings:SmtpServer"],
                        int.Parse(_configuration["EmailSettings:Port"]),
                        MailKit.Security.SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]
                    );

                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            string subject = "Welcome to Appointment Management System";

            string htmlBody1 = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
                <h2 style='color: #4CAF50; text-align: center;'>Welcome to Appointment Management System</h2>
                <p style='font-size: 16px; color: #555;'>Dear {userName},</p>
                <p style='font-size: 16px; color: #555;'>
                    Thank you for registering with our system. We are excited to have you on board!
                </p>
                <p style='font-size: 16px; color: #555;'>
                    If you have any questions, feel free to contact our support team.
                </p>
                <p style='font-size: 16px; color: #555;'><strong>Best regards,</strong><br>Appointment Management Team</p>
            </div>";

            await SendEmailAsync(toEmail, subject, htmlBody1);
        }

        public async Task SendCustomerAdminCredentialsEmailAsync(string toEmail, string password)
        {
            string subject = "Your HR Admin Account Credentials";

            string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
            <h2 style='color: #333; text-align: center;'>Welcome to Appointment Management</h2>
            <p style='font-size: 16px; color: #555;'>Hello,</p>
            <p style='font-size: 16px; color: #555;'>
                Your HR Admin account has been successfully created. Below are your login credentials:
            </p>
            <div style='background-color: #fff; padding: 15px; border-radius: 5px; border: 1px solid #ccc;'>
                <p style='font-size: 16px;'><strong>Email:</strong> {toEmail}</p>
                <p style='font-size: 16px;'><strong>Password:</strong> {password}</p>
            </div>
            <p style='font-size: 16px; color: #555;'>
                Please log in and change your password immediately for security reasons.
            </p>
            <p style='text-align: center; margin-top: 20px;'>
                <a href='https://yourcompany.com/login' style='background-color: #007bff; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-size: 16px;'>Login Now</a>
            </p>
            <p style='font-size: 16px; color: #555;'><strong>Best regards,</strong><br>Your Company Team</p>
        </div>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }
    }
}
