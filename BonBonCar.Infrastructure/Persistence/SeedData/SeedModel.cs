using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Persistence.SeedData
{
    public static class SeedModel
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            // Nếu đã có model thì không seed nữa
            if (await dbContext.Models.AnyAsync())
                return;

            // Lấy Brand theo Name
            var toyota = await dbContext.Brands.FirstAsync(x => x.Name == "Toyota");
            var honda = await dbContext.Brands.FirstAsync(x => x.Name == "Honda");
            var hyundai = await dbContext.Brands.FirstAsync(x => x.Name == "Hyundai");
            var kia = await dbContext.Brands.FirstAsync(x => x.Name == "Kia");
            var mazda = await dbContext.Brands.FirstAsync(x => x.Name == "Mazda");
            var ford = await dbContext.Brands.FirstAsync(x => x.Name == "Ford");
            var vinfast = await dbContext.Brands.FirstAsync(x => x.Name == "VinFast");

            var models = new List<Model>
            {
                // ===== TOYOTA =====
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Wigo", VehicleType = EnumCarType.Mini },
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Vios", VehicleType = EnumCarType.SedanB },
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Corolla Altis", VehicleType = EnumCarType.SedanC },
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Camry", VehicleType = EnumCarType.SedanC },
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Raize", VehicleType = EnumCarType.SuvB },
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Corolla Cross", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Fortuner", VehicleType = EnumCarType.Suv7 },
                new() { Id = Guid.NewGuid(), BrandId = toyota.Id, Name = "Innova", VehicleType = EnumCarType.Mpv7 },

                // ===== HONDA =====
                new() { Id = Guid.NewGuid(), BrandId = honda.Id, Name = "Brio", VehicleType = EnumCarType.Mini },
                new() { Id = Guid.NewGuid(), BrandId = honda.Id, Name = "City", VehicleType = EnumCarType.SedanB },
                new() { Id = Guid.NewGuid(), BrandId = honda.Id, Name = "Civic", VehicleType = EnumCarType.SedanC },
                new() { Id = Guid.NewGuid(), BrandId = honda.Id, Name = "Accord", VehicleType = EnumCarType.SedanC },
                new() { Id = Guid.NewGuid(), BrandId = honda.Id, Name = "HR-V", VehicleType = EnumCarType.SuvB },
                new() { Id = Guid.NewGuid(), BrandId = honda.Id, Name = "CR-V", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = honda.Id, Name = "BR-V", VehicleType = EnumCarType.Mpv7 },

                // ===== HYUNDAI =====
                new() { Id = Guid.NewGuid(), BrandId = hyundai.Id, Name = "i10", VehicleType = EnumCarType.Mini },
                new() { Id = Guid.NewGuid(), BrandId = hyundai.Id, Name = "Accent", VehicleType = EnumCarType.SedanB },
                new() { Id = Guid.NewGuid(), BrandId = hyundai.Id, Name = "Elantra", VehicleType = EnumCarType.SedanC },
                new() { Id = Guid.NewGuid(), BrandId = hyundai.Id, Name = "Creta", VehicleType = EnumCarType.SuvB },
                new() { Id = Guid.NewGuid(), BrandId = hyundai.Id, Name = "Tucson", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = hyundai.Id, Name = "Santa Fe", VehicleType = EnumCarType.Suv7 },
                new() { Id = Guid.NewGuid(), BrandId = hyundai.Id, Name = "Stargazer", VehicleType = EnumCarType.Mpv7 },

                // ===== KIA =====
                new() { Id = Guid.NewGuid(), BrandId = kia.Id, Name = "Morning", VehicleType = EnumCarType.Mini },
                new() { Id = Guid.NewGuid(), BrandId = kia.Id, Name = "K3", VehicleType = EnumCarType.SedanB },
                new() { Id = Guid.NewGuid(), BrandId = kia.Id, Name = "K5", VehicleType = EnumCarType.SedanC },
                new() { Id = Guid.NewGuid(), BrandId = kia.Id, Name = "Sonet", VehicleType = EnumCarType.SuvB },
                new() { Id = Guid.NewGuid(), BrandId = kia.Id, Name = "Seltos", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = kia.Id, Name = "Sorento", VehicleType = EnumCarType.Suv7 },
                new() { Id = Guid.NewGuid(), BrandId = kia.Id, Name = "Carnival", VehicleType = EnumCarType.Mpv7 },

                // ===== MAZDA =====
                new() { Id = Guid.NewGuid(), BrandId = mazda.Id, Name = "Mazda 2", VehicleType = EnumCarType.SedanB },
                new() { Id = Guid.NewGuid(), BrandId = mazda.Id, Name = "Mazda 3", VehicleType = EnumCarType.SedanC },
                new() { Id = Guid.NewGuid(), BrandId = mazda.Id, Name = "CX-3", VehicleType = EnumCarType.SuvB },
                new() { Id = Guid.NewGuid(), BrandId = mazda.Id, Name = "CX-5", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = mazda.Id, Name = "CX-8", VehicleType = EnumCarType.Suv7 },

                // ===== FORD =====
                new() { Id = Guid.NewGuid(), BrandId = ford.Id, Name = "EcoSport", VehicleType = EnumCarType.SuvB },
                new() { Id = Guid.NewGuid(), BrandId = ford.Id, Name = "Territory", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = ford.Id, Name = "Everest", VehicleType = EnumCarType.Suv7 },
                new() { Id = Guid.NewGuid(), BrandId = ford.Id, Name = "Ranger", VehicleType = EnumCarType.Suv7 },

                // ===== VINFAST =====
                new() { Id = Guid.NewGuid(), BrandId = vinfast.Id, Name = "Fadil", VehicleType = EnumCarType.Mini },
                new() { Id = Guid.NewGuid(), BrandId = vinfast.Id, Name = "VF 5", VehicleType = EnumCarType.SuvB },
                new() { Id = Guid.NewGuid(), BrandId = vinfast.Id, Name = "VF 6", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = vinfast.Id, Name = "VF 8", VehicleType = EnumCarType.SuvC },
                new() { Id = Guid.NewGuid(), BrandId = vinfast.Id, Name = "VF 9", VehicleType = EnumCarType.Suv7 }
            };

            await dbContext.Models.AddRangeAsync(models);
            await dbContext.SaveChangesAsync();
        }
    }
}
