using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.CmdModels.CarCmdModels
{
    public class CreateCarCmdModel
    {
        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }
        [Required]
        [StringLength(20)]
        public string? LicensePlate { get; set; }
    }
}
