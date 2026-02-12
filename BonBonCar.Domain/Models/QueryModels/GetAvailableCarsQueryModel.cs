namespace BonBonCar.Domain.Models.QueryModels
{
    public class GetAvailableCarsQueryModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}