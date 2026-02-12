using BonBonCar.Domain.Enums.Car;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class CarModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? LicensePlate { get; set; }
        public decimal PricePerDay { get; set; }
        public EnumCarStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
