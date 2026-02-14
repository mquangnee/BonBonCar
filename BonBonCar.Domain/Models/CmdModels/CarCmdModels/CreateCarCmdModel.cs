using Microsoft.AspNetCore.Http;

namespace BonBonCar.Domain.Models.CmdModels.CarCmdModels
{
    public class CreateCarCmdModel
    {
        public Guid ModelId { get; set; }
        /// <summary>
        /// Location code (e.g. HN, HCM,...).
        /// </summary>
        public string? Location { get; set; }
        public int Year { get; set; }
        public string? LicensePlate { get; set; }
        public string? PickupAddress { get; set; }
        public IList<string>? Features { get; set; }
        public IList<IFormFile>? Images { get; set; }
        public string? Prices { get; set; }
    }
}
