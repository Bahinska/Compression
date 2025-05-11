using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using ServerAPI.Models;

namespace ServerAPI.Services
{
    public class EmailSender : IEmailSenderExtended
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IOptions<AuthMessageSenderOptions> _options;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(Options.MailPass))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.MailPass, subject, message, toEmail);
        }

        public async Task Execute(string pass, string subject, string message, string toEmail)
        {
            var fromMail = "marharyta.bahinska@lnu.edu.ua";
            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromMail, pass)
            };

            await client.SendMailAsync(
                new MailMessage(from: fromMail,
                                to: toEmail,
                                subject,
                                message)
                { IsBodyHtml = true });
        }

        public async Task SendEmailWithImageAsync(string toEmail, string subject, byte[] imageBytes)
        {
            if (string.IsNullOrEmpty(Options.MailPass))
            {
                throw new Exception("Null SendGridKey");
            }

            var fromMail = "marharyta.bahinska@lnu.edu.ua";
            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromMail, Options.MailPass)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromMail),
                Subject = subject,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            var inline = new LinkedResource(new MemoryStream(imageBytes))
            {
                ContentId = Guid.NewGuid().ToString(),
                ContentType = new ContentType("image/jpeg")
            };

            string htmlBody = $"New object has been detected! <br><img src=\"cid:{inline.ContentId}\"/>";
            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            avHtml.LinkedResources.Add(inline);
            mailMessage.AlternateViews.Add(avHtml);

            await client.SendMailAsync(mailMessage);
        }
    }
}