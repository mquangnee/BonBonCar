using BonBonCar.Domain.Enums.Car;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class RentalCarModel
    {
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public int Year { get; set; }
        public string? LicensePlate { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<string>? Features { get; set; }
        public List<CarPriceModel>? Prices { get; set; }
        public List<string>? Images { get; set; } 
        public EnumCarStatus Status { get; set; }
    }
}
