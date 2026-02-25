namespace BonBonCar.Domain.Models.ServiceModel.VnpayService
{
    public class VnpayValidateResult
    {
        public bool IsValidSignature { get; set; }
        public string? TxnRef { get; set; }
        public string? ResponseCode { get; set; }
        public string? TransactionNo { get; set; }
        public long? Amount { get; set; } // amount * 100
        public string RawQueryString { get; set; } = string.Empty;

        public VnpayValidateResult() { }

        public VnpayValidateResult(
            bool isValidSignature,
            string? txnRef,
            string? responseCode,
            string? transactionNo,
            long? amount,
            string rawQueryString)
        {
            IsValidSignature = isValidSignature;
            TxnRef = txnRef;
            ResponseCode = responseCode;
            TransactionNo = transactionNo;
            Amount = amount;
            RawQueryString = rawQueryString;
        }
    }
}
