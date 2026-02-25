namespace BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels
{
    public class CancelRentalResponse
    {
        public Guid RentalOrderId { get; set; }
        public string Status { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
    }
}
