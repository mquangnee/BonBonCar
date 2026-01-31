using BonBonCar.Domain.IService;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace BonBonCar.Infrastructure.Services.Sender
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly string _resourceRoot;

        public EmailTemplate(IWebHostEnvironment env)
        {
            _resourceRoot = Path.Combine(env.ContentRootPath, "Resources");
        }

        public async Task<string> GetOtpEmailBodyAsync(string otp, string title, string previewLine, string messageLine, string userName,CancellationToken cancellationToken)
        {
            var html = await LoadTemplateAsync("OtpEmail.html", cancellationToken);

            // Replace placeholders (HTML-encode text fields to avoid breaking HTML)
            html = ReplaceToken(html, "PreviewLine", previewLine, encode: true);
            html = ReplaceToken(html, "Title", title, encode: true);
            html = ReplaceToken(html, "UserName", userName, encode: true);
            html = ReplaceToken(html, "MessageLine", messageLine, encode: true);
            html = ReplaceToken(html, "OTP", otp, encode: true);

            return html;
        }

        public async Task<string> GetChangePasswordSuccessEmailBodyAsync(string userName, string time, CancellationToken cancellationToken)
        {
            var html = await LoadTemplateAsync("ChangePasswordSuccess.html", cancellationToken);

            html = ReplaceToken(html, "UserName", userName, encode: true);
            html = ReplaceToken(html, "Time", time, encode: true);

            return html;
        }

        private static string ReplaceToken(string html, string key, string value, bool encode)
        {
            var token = $"[{key}]";
            var safe = encode ? System.Net.WebUtility.HtmlEncode(value ?? string.Empty) : (value ?? string.Empty);
            return html.Replace(token, safe, StringComparison.Ordinal);
        }

        private async Task<string> LoadTemplateAsync(string fileName, CancellationToken cancellationToken)
        {
            var path = Path.Combine(_resourceRoot, fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Email template not found: {path}");

            return await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);
        }


    }
}
