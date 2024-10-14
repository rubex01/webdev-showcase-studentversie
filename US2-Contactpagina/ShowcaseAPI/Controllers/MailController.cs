using Microsoft.AspNetCore.Mvc;
using ShowcaseAPI.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowcaseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        // POST api/<MailController>
        [HttpPost]
        public ActionResult Post([Bind("FirstName, LastName, Email, Phone")] Contactform form)
        {
            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
            var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "2525");
            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var message = new StringBuilder();
            message.AppendLine($"New Contact Form Submission:");
            message.AppendLine($"Name: {form.FirstName} {form.LastName}");
            message.AppendLine($"Email: {form.Email}");
            message.AppendLine($"Phone: {form.Phone}");

            try
            {
                client.Send(form.Email, "rbpouwen@gmail.com", "Contact verzoek", message.ToString());
                Console.WriteLine("Email sent successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return StatusCode(500, "Failed to send email");
            }
        }
    }
}
