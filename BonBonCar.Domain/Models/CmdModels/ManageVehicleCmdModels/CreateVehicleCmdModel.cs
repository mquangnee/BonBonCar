using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.CmdModels.ManageVehicleCmdModels
{
    public class CreateVehicleCmdModel
    {
        [Required]
        [StringLength(50)]
        public string? Brand { get; set; }
        [Required]
        [StringLength(50)]
        public string? Model { get; set; }
        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }
        [Required]
        [StringLength(20)]
        public string? LicensePlate { get; set; }
        [Required]
        public decimal PricePerDay { get; set; }
    }
}
