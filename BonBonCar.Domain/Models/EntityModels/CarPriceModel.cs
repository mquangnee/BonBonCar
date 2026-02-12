using BonBonCar.Domain.Enums.Car;
using System.Text.Json.Serialization;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class CarPriceModel
    {
        [JsonPropertyName("rentalDuration")]
        public EnumRentalDuration RentalDuration { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
