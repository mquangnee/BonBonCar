namespace BonBonCar.Domain.Models.EntityModels
{
    public class CarModel
    {
        public Guid CarId { get; set; }
        public string CarName { get; set; } = default!;
        public string? PickupAddress { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public decimal Price4h { get; set; }
        public decimal Price24h { get; set; }
    }
}
