namespace BonBonCar.Domain.Models.QueryModels.RentalOrders
{
    public class MyActiveRentalsResult
    {
        public IList<MyActiveRentalsItem> Items { get; set; } = new List<MyActiveRentalsItem>();
    }

    public class MyActiveRentalsItem
    {
        public Guid RentalOrderId { get; set; }
        public Guid CarId { get; set; }
        public string CarName { get; set; } = "";
        public string? PrimaryImageUrl { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }

        public decimal DepositAmount { get; set; }

        public decimal TotalRentalFee { get; set; }
        public decimal PaidRentalFeeAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public bool HasUnfinishedRentalFeePayment { get; set; }

        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
