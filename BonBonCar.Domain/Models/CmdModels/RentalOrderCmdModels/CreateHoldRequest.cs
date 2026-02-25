namespace BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels
{
    public class CreateHoldRequest
    {
        public Guid CarId { get; set; }

        public DateTime PickupAt { get; set; }

        public DateTime ReturnAt { get; set; }

        public CreateHoldRequest() { }

        public CreateHoldRequest(Guid carId, DateTime pickupAt, DateTime returnAt)
        {
            CarId = carId;
            PickupAt = pickupAt;
            ReturnAt = returnAt;
        }
    }
}
