namespace BonBonCar.Domain.Models.EntityModels
{
    public class CarForRentModel
    {
        public Guid CarId { get; set; }

        public string CarName { get; set; } = default!;
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public int Year { get; set; }

        public string? Location { get; set; }
        public string? PickupAddress { get; set; }

        public DateTime PickupDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }

        public string? PrimaryImageUrl { get; set; }
        public IList<string> Images { get; set; } = new List<string>();

        public IList<string> Features { get; set; } = new List<string>();

        public decimal Price4h { get; set; }
        public decimal Price8h { get; set; }
        public decimal Price12h { get; set; }
        public decimal Price24h { get; set; }

        public int TotalHours { get; set; }
        public decimal TotalPrice { get; set; }
        public IList<PriceBreakdownItem> Breakdown { get; set; } = new List<PriceBreakdownItem>();
    }
    public sealed class PriceBreakdownItem
    {
        public int PackageHours { get; set; }   // 4/8/12/24
        public int Quantity { get; set; }       // số gói
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}
