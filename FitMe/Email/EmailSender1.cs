using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace FitMe.NewFolder
{
    public class EmailSender11
    {
        private readonly string _apiKey;

        public EmailSender11(IConfiguration config)
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("SendGrid API Key is missing from environment variables!");
            }
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("mohammed.ali.abdalnabi@gmail.com", "FitMe App");
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, null, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
