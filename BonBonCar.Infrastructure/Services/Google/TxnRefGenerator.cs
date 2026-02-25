namespace BonBonCar.Infrastructure.Services.GoogleDocumentAI
{
    public static class TxnRefGenerator
    {
        public static string NewTxnRef(Guid rentalOrderId)
        => $"{DateTime.Now:yyyyMMddHHmmss}-{rentalOrderId.ToString("N")[..12]}";
    }
}
