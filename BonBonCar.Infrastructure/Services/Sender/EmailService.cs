using BonBonCar.Domain.IService;
using BonBonCar.Infrastructure.Services.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace BonBonCar.Infrastructure.Services.Sender
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _settings;
        private readonly IWebHostEnvironment _env;

        public EmailService(IOptions<EmailSetting> options, IWebHostEnvironment env)
        {
            _settings = options.Value;
            _env = env;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.From, _settings.Password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.From, "BonBonCar"),
                Subject = subject,
                IsBodyHtml = true
            };
            message.To.Add(email);

            var htmlView = AlternateView.CreateAlternateViewFromString(
                htmlBody,
                null,
                MediaTypeNames.Text.Html
            );

            var logoPath = Path.Combine(_env.ContentRootPath, "wwwroot", "images", "logo_bonboncar.jpeg");

            if (File.Exists(logoPath))
            {
                var logo = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "bonboncar-logo",
                    TransferEncoding = TransferEncoding.Base64
                };
                htmlView.LinkedResources.Add(logo);
            }
            else
            {
                htmlBody = htmlBody.Replace("cid:bonboncar-logo", "https://bonboncar.vn/images/logo_bonboncar.jpeg");
                htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            }

            message.AlternateViews.Add(htmlView);

            await client.SendMailAsync(message);
        }
    }
}
