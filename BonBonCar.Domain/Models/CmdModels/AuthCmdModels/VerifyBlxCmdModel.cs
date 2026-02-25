using Microsoft.AspNetCore.Http;

namespace BonBonCar.Domain.Models.CmdModels.AuthCmdModels
{
    public class VerifyBlxCmdModel
    {
        public IFormFile FrontImage { get; set; } = default!;
    }
}
