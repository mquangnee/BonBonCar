using BonBonCar.Domain.Enums.User;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class UserModel
    {
        public string? FullName { get; set; }
        public EnumGender Gender { get; set; } = EnumGender.Unknown;
        public DateOnly DateOfBirth { get; set; }
        [StringLength(300)]
        public string? Address { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}
