using BonBonCar.Domain.Enums.User;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class UserDocument
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public EnumDocumentType DocumentType { get; set; }
        [Required]
        [StringLength(50)]
        public string? DocumentNumber { get; set; }
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public DateOnly IssuedDate { get; set; }
        [Required]
        public DateOnly ExpiredDate { get; set; }
        [Required]
        [StringLength(500)]
        public string? FrontImageUrl { get; set; }
        [Required]
        [StringLength(500)]
        public string? BackImageUrl { get; set; }
        [Required]
        public EnumDocumentStatus Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
