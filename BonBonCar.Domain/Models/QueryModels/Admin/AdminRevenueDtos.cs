using BonBonCar.Domain.Enums.Payment;

namespace BonBonCar.Domain.Models.QueryModels.Admin
{
    public class RevenueSummaryItem
    {
        public EnumPaymentPurpose Purpose { get; set; }
        public decimal Amount { get; set; }
    }

    public class RevenueSummaryModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal TotalDeposit { get; set; }
        public decimal TotalRentalFee { get; set; }
        public decimal TotalRevenue => TotalDeposit + TotalRentalFee;

        public IList<RevenueSummaryItem> ByPurpose { get; set; } = new List<RevenueSummaryItem>();
    }
}

