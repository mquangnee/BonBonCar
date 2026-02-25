using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.ServiceModel.VnpayService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BonBonCar.Infrastructure.Payments.Vnpay
{
    public class VnpayGateway : IVnpayGateway
    {
        private readonly VnpayOptions _options;

        public VnpayGateway(IOptions<VnpayOptions> options)
        {
            _options = options.Value;
        }

        public string BuildPaymentUrl(VnpayBuildUrlRequest req)
        {
            long amount = ((long)Math.Round(req.AmountVnd, 0, MidpointRounding.AwayFromZero)) * 100;

            var vnp = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"]    = _options.Version,
                ["vnp_Command"]    = _options.Command,
                ["vnp_TmnCode"]    = _options.TmnCode,
                ["vnp_Amount"]     = amount.ToString(CultureInfo.InvariantCulture),
                ["vnp_CurrCode"]   = _options.CurrCode,
                ["vnp_TxnRef"]     = req.TxnRef,
                ["vnp_OrderInfo"]  = SanitizeOrderInfo(req.OrderInfo),
                ["vnp_OrderType"]  = "other",
                ["vnp_Locale"]     = _options.Locale,
                ["vnp_ReturnUrl"]  = req.ReturnUrl,
                ["vnp_IpAddr"]     = NormalizeIp(req.IpAddress),
                ["vnp_CreateDate"] = req.CreateDateLocal.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                ["vnp_ExpireDate"] = req.ExpireDateLocal.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            };

            if (!string.IsNullOrWhiteSpace(req.IpnUrl))
                vnp["vnp_IpnUrl"] = req.IpnUrl.Trim();

            string canonical = BuildQueryString(vnp);
            string secureHash = HmacSha512(_options.HashSecret, canonical);
            return $"{_options.PaymentUrl}?{canonical}&vnp_SecureHash={secureHash}";
        }

        public VnpayValidateResult ValidateCallback(IQueryCollection query)
        {
            string rawQueryString = query.ToString() ?? string.Empty;
            string? receivedHash = query["vnp_SecureHash"].FirstOrDefault();

            var vnp = new SortedDictionary<string, string>(StringComparer.Ordinal);

            foreach (var kv in query)
            {
                var key = kv.Key;

                if (key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase)) continue;
                if (key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase)) continue;

                var val = kv.Value.ToString();
                if (!string.IsNullOrEmpty(val))
                    vnp[key] = val;
            }

            string canonical = BuildQueryString(vnp);
            string expected = HmacSha512(_options.HashSecret, canonical);

            bool ok = !string.IsNullOrEmpty(receivedHash)
                      && string.Equals(receivedHash, expected, StringComparison.OrdinalIgnoreCase);

            return new VnpayValidateResult
            {
                IsValidSignature = ok,
                TxnRef = query["vnp_TxnRef"].FirstOrDefault(),
                ResponseCode = query["vnp_ResponseCode"].FirstOrDefault(),
                TransactionNo = query["vnp_TransactionNo"].FirstOrDefault(),
                Amount = long.TryParse(query["vnp_Amount"].FirstOrDefault(), NumberStyles.Any, CultureInfo.InvariantCulture, out var a) ? a : null,
                RawQueryString = rawQueryString
            };
        }

        private static string BuildQueryString(SortedDictionary<string, string> dict)
        {
            var sb = new StringBuilder();
            foreach (var (k, v) in dict)
            {
                if (string.IsNullOrEmpty(v)) continue;
                if (sb.Length > 0) sb.Append('&');
                sb.Append(WebUtility.UrlEncode(k));
                sb.Append('=');
                sb.Append(WebUtility.UrlEncode(v));
            }
            return sb.ToString();
        }

        private static string HmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);

            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static string NormalizeIp(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return "127.0.0.1";
            ip = ip.Trim();
            if (ip == "::1") return "127.0.0.1";
            if (ip.StartsWith("::ffff:", StringComparison.OrdinalIgnoreCase)) return ip.Substring("::ffff:".Length);
            return ip;
        }

        private static string SanitizeOrderInfo(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s ?? string.Empty;
            return s.Replace("\r", " ").Replace("\n", " ").Trim();
        }
    }
}