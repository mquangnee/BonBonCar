using BonBonCar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BonBonCar.Infrastructure.Persistence.SeedData
{
    public static class SeedAdminUser
    {
        public static async Task SeedAsync(DataContext db, IServiceProvider serviceProvider)
        {
            _ = db;

            using var scope = serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("SeedAdminUser");

            var userManager = sp.GetRequiredService<UserManager<User>>();
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            const string adminEmail = "admin@bonboncar.vn";
            const string adminPassword = "Admin@123456";
            const string adminRole = "Admin";

            try
            {
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError("Không thể tạo role Admin: {Errors}",
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        return;
                    }
                }
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (!createResult.Succeeded)
                    {
                        logger.LogError("Tạo admin user thất bại: {Errors}",
                            string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        return;
                    }
                }

                if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                {
                    var addRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
                    if (!addRoleResult.Succeeded)
                    {
                        logger.LogError("Gán role Admin thất bại: {Errors}",
                            string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                        return;
                    }
                }

                logger.LogInformation("Seed admin user thành công");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi seed admin user");
            }
        }
    }
}