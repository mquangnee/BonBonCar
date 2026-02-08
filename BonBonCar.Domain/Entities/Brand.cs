using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class Brand
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
