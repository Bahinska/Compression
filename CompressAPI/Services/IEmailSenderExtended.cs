using Microsoft.AspNetCore.Identity.UI.Services;

namespace ServerAPI.Services
{
    public interface IEmailSenderExtended : IEmailSender
    {
        Task SendEmailWithImageAsync(string toEmail, string subject, byte[] imageBytes);
    }
}
