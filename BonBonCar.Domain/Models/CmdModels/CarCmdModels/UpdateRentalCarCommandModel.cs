using Microsoft.AspNetCore.Http;

namespace BonBonCar.Domain.Models.CmdModels.CarCmdModels
{
    public class UpdateRentalCarCommandModel
    {
        public Guid CarId { get; set; }
        public IList<string>? Features { get; set; }
        public IList<IFormFile>? Images { get; set; }
        public string? Prices { get; set; }
    }
}
