using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.ServiceModel.VnpayService;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BonBonCar.Infrastructure.Services.Payments
{
    public sealed class VnpayTransactionService : IVnpayTransactionService
    {
        private readonly HttpClient _http;
        private readonly VnpayOptions _options;

        public VnpayTransactionService(HttpClient http, IOptions<VnpayOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<VnpayRefundResult> RefundAsync(VnpayRefundRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            long amount = ((long)Math.Round(request.AmountVnd, 0, MidpointRounding.AwayFromZero)) * 100;

            var payload = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_RequestId"] = request.RequestId,
                ["vnp_Version"] = _options.Version,
                ["vnp_Command"] = "refund",
                ["vnp_TmnCode"] = _options.TmnCode,
                ["vnp_TransactionType"] = request.TransactionType,
                ["vnp_TxnRef"] = request.TxnRef,
                ["vnp_Amount"] = amount.ToString(CultureInfo.InvariantCulture),
                ["vnp_TransactionNo"] = request.TransactionNo ?? string.Empty,
                ["vnp_TransactionDate"] = request.TransactionDate,
                ["vnp_CreateBy"] = request.CreateBy,
                ["vnp_CreateDate"] = request.CreateDate,
                ["vnp_IpAddr"] = request.IpAddr,
                ["vnp_OrderInfo"] = request.OrderInfo
            };

            string dataToSign =
                $"{payload["vnp_RequestId"]}|{payload["vnp_Version"]}|{payload["vnp_Command"]}|{payload["vnp_TmnCode"]}|{payload["vnp_TransactionType"]}|{payload["vnp_TxnRef"]}|{payload["vnp_Amount"]}|{payload["vnp_TransactionNo"]}|{payload["vnp_TransactionDate"]}|{payload["vnp_CreateBy"]}|{payload["vnp_CreateDate"]}|{payload["vnp_IpAddr"]}|{payload["vnp_OrderInfo"]}";

            payload["vnp_SecureHash"] = HmacSha512(_options.HashSecret, dataToSign);

            var url = string.IsNullOrWhiteSpace(_options.TransactionApiUrl) ? "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction" : _options.TransactionApiUrl.Trim();
            using var msg = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            using var res = await _http.SendAsync(msg, ct).ConfigureAwait(false);
            var raw = await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            if (!res.IsSuccessStatusCode)
            {
                return new VnpayRefundResult
                {
                    IsSuccess = false,
                    IsValidSignature = false,
                    ResponseCode = ((int)res.StatusCode).ToString(CultureInfo.InvariantCulture),
                    Message = string.IsNullOrWhiteSpace(raw) ? "HTTP error" : raw,
                    Raw = raw
                };
            }

            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(raw) ?? new();
            string Get(string key) => dict.TryGetValue(key, out var v) ? v.ToString() ?? "" : "";

            var responseCode = Get("vnp_ResponseCode");
            var message = Get("vnp_Message");
            var transactionStatus = Get("vnp_TransactionStatus");

            var receivedHash = Get("vnp_SecureHash");
            bool verified = false;
            if (!string.IsNullOrWhiteSpace(receivedHash))
            {
                string responseId = Get("vnp_ResponseId");
                string command = Get("vnp_Command");
                string tmnCode = Get("vnp_TmnCode");
                string txnRef = Get("vnp_TxnRef");
                string amountStr = Get("vnp_Amount");
                string bankCode = Get("vnp_BankCode");
                string payDate = Get("vnp_PayDate");
                string txnNo = Get("vnp_TransactionNo");
                string txnType = Get("vnp_TransactionType");
                string orderInfo = Get("vnp_OrderInfo");
                string promoCode = Get("vnp_PromotionCode");
                string promoAmount = Get("vnp_PromotionAmount");

                string verifyData = $"{responseId}|{command}|{responseCode}|{message}|{tmnCode}|{txnRef}|{amountStr}|{bankCode}|{payDate}|{txnNo}|{txnType}|{transactionStatus}|{orderInfo}|{promoCode}|{promoAmount}";

                var expected = HmacSha512(_options.HashSecret, verifyData);
                verified = string.Equals(receivedHash, expected, StringComparison.OrdinalIgnoreCase);
            }

            return new VnpayRefundResult
            {
                IsSuccess = string.Equals(responseCode, "00", StringComparison.Ordinal),
                IsValidSignature = verified,
                ResponseCode = string.IsNullOrWhiteSpace(responseCode) ? "99" : responseCode,
                Message = string.IsNullOrWhiteSpace(message) ? "Unknown" : message,
                TransactionStatus = string.IsNullOrWhiteSpace(transactionStatus) ? null : transactionStatus,
                Raw = raw
            };
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
    }
}

