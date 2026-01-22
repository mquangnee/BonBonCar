using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Infrastructure.Identity
{
    public class IdentitySeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles =
            {
                AppRoles.Admin,
                AppRoles.User
            };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}
