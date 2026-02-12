using Microsoft.AspNetCore.Http;

namespace BonBonCar.Domain.Models.CmdModels.CarCmdModels
{
    public class CreateCarCmdModel
    {
        public Guid ModelId { get; set; }
        public int Year { get; set; }
        public string? LicensePlate { get; set; }
        public string? Location { get; set; }
        public IList<string>? Features { get; set; }
        public IList<IFormFile>? Images { get; set; }
        public string? Prices { get; set; }
    }
}
