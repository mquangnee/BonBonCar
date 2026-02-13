using BonBonCar.Domain.Enums.Car;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonBonCar.Domain.Entities
{
    public class RentalContract
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid RentalOrderId { get; set; }
        [Required]
        [StringLength(30)]
        public string? ContractNumber { get; set; }
        [Required]
        public DateTime SignedAt { get; set; } = DateTime.Now;
        [Required]
        [StringLength(500)]
        public string? FileUrl { get; set; }
        [Required]
        public RentalContractStatus Status { get; set; } = RentalContractStatus.Pending;
        [ForeignKey(nameof(RentalOrderId))]
        public RentalOrder? RentalOrder { get; set; } 
    }
}
