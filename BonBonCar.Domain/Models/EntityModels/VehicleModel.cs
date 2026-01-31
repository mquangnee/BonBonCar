using BonBonCar.Domain.Enums.Vehicle;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class VehicleModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? LicensePlate { get; set; }
        public decimal PricePerDay { get; set; }
        public EnumVehicleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
