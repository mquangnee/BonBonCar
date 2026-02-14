using BonBonCar.Domain.Enums.Car;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class RentalCarModel
    {
        public Guid CarId { get; set; }
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public int Year { get; set; }
        /// <summary>
        /// Location code (e.g. HN, HCM,...).
        /// </summary>
        public string? Location { get; set; }
        public string? PickupAddress { get; set; }
        public string? LicensePlate { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<string>? Features { get; set; }
        public List<BasePriceModel>? BasePrices { get; set; }
        public List<CarPriceModel>? Prices { get; set; }
        public List<string>? Images { get; set; } 
        public EnumCarStatus Status { get; set; }
    }
}
