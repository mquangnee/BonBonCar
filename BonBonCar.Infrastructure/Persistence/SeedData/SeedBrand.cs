using BonBonCar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Persistence.SeedData
{
    public static class SeedBrand
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            // Nếu đã có brand thì không seed nữa
            if (await dbContext.Brands.AnyAsync())
                return;

            var brands = new List<Brand>
            {
                new() { Id = Guid.NewGuid(), Name = "Toyota" },
                new() { Id = Guid.NewGuid(), Name = "Honda" },
                new() { Id = Guid.NewGuid(), Name = "Hyundai" },
                new() { Id = Guid.NewGuid(), Name = "Kia" },
                new() { Id = Guid.NewGuid(), Name = "Mazda" },
                new() { Id = Guid.NewGuid(), Name = "Ford" },
                new() { Id = Guid.NewGuid(), Name = "VinFast" }
            };

            await dbContext.Brands.AddRangeAsync(brands);
            await dbContext.SaveChangesAsync();
        }
    }
}
