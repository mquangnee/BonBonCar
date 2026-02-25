namespace BonBonCar.Domain.Models.QueryModels.CarQueryModel
{
    public class GetAvailableCarsQueryModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}