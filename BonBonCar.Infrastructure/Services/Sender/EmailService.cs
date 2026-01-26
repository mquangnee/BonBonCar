using BonBonCar.Domain.IService;
using BonBonCar.Infrastructure.Services.Model;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BonBonCar.Infrastructure.Services.Sender
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _settings;

        public EmailService(IOptions<EmailSetting> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.From, _settings.Password),
                EnableSsl = true
            };
            var mailMessage = new MailMessage(_settings.From, email, subject, body)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(mailMessage);
        }
    }
}
