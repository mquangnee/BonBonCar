using Microsoft.AspNetCore.Http;

namespace BonBonCar.Domain.Models.CmdModels.AuthCmdModels
{
    public class VerifyCccdCmdModel
    {
        public string? DocumentNumber { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? PlaceOfOrigin { get; set; }
        public string? PlaceOfResidence { get; set; }
        public IFormFile FrontImage { get; set; } = default!;
    }
}
