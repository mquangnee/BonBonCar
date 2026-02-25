using BonBonCar.Domain.Enums.Car;

namespace BonBonCar.Domain.Models.QueryModels.Admin
{
    public class CarItemModel
    {
        public Guid CarId { get; set; }
        public string CarName { get; set; } = string.Empty;
        public EnumCarStatus Status { get; set; }
        public string? PickupAddress { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ListCarsModel
    {
        public IList<CarItemModel> Items { get; set; } = new List<CarItemModel>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

