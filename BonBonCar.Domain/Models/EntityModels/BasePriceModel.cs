using BonBonCar.Domain.Enums.Car;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class BasePriceModel
    {
        public EnumCarType VehicleType { get; set; }
        public EnumRentalDuration RentalDuration { get; set; }
        public decimal BasePrice { get; set; }
    }
}
