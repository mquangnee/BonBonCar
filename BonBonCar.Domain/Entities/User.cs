using BonBonCar.Domain.Enums.User;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities;

public class User : IdentityUser<Guid>
{
    [StringLength(100)]
    public string? FullName { get; set; }
    public EnumGender Gender { get; set; } = EnumGender.Unknown;
    public DateOnly DateOfBirth { get; set; }
    [StringLength(300)]
    public string? Address { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}