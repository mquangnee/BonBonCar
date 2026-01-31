using BonBonCar.Domain.IService;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace BonBonCar.Infrastructure.Services.Sender
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly string _templatePath;

        public EmailTemplate(IWebHostEnvironment env)
        {
            _templatePath = Path.Combine(env.ContentRootPath, "Resources", "OtpEmail.html");
        }

        public async Task<string> GetOtpEmailBodyAsync(string otp, string title, string previewLine, string messageLine, string userName,CancellationToken cancellationToken)
        {
            if (!File.Exists(_templatePath))
                throw new FileNotFoundException($"OTP email template not found: {_templatePath}");

            var html = await File.ReadAllTextAsync(_templatePath, Encoding.UTF8, cancellationToken);

            // Replace placeholders (HTML-encode text fields to avoid breaking HTML)
            html = ReplaceToken(html, "PreviewLine", previewLine, encode: true);
            html = ReplaceToken(html, "Title", title, encode: true);
            html = ReplaceToken(html, "UserName", userName, encode: true);
            html = ReplaceToken(html, "MessageLine", messageLine, encode: true);
            html = ReplaceToken(html, "OTP", otp, encode: true);

            return html;
        }

        private static string ReplaceToken(string html, string key, string value, bool encode)
        {
            var token = $"[{key}]";
            var safe = encode ? System.Net.WebUtility.HtmlEncode(value ?? string.Empty) : (value ?? string.Empty);
            return html.Replace(token, safe, StringComparison.Ordinal);
        }
    }
}
