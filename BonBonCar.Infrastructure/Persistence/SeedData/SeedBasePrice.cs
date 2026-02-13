using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Persistence.SeedData
{
    public static class SeedBasePrice
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            // Seed 1 lần
            if (await dbContext.BasePrices.AnyAsync())
                return;

            var items = new List<BasePrices>
            {
                // ===== MINI (ước tính, vì ảnh không có dòng Mini) =====
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mini, RentalDuration = EnumRentalDuration.hour4,  BasePrice = 350000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mini, RentalDuration = EnumRentalDuration.hour8,  BasePrice = 450000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mini, RentalDuration = EnumRentalDuration.hour12, BasePrice = 500000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mini, RentalDuration = EnumRentalDuration.hour24, BasePrice = 650000m, IsActive = true },

                // ===== SEDAN B (trung bình từ: Accent/City/Vios/Mazda2 trong ảnh) =====
                // 4h: (400+450+450+450)/4 = 437.500  -> làm tròn 440.000
                // 8h: (500+560+560+530)/4 = 537.500  -> làm tròn 540.000
                // 12h:(550+640+640+600)/4 = 607.500  -> làm tròn 610.000
                // 24h:(700+800+800+750)/4 = 762.500  -> làm tròn 760.000
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanB, RentalDuration = EnumRentalDuration.hour4,  BasePrice = 440000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanB, RentalDuration = EnumRentalDuration.hour8,  BasePrice = 540000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanB, RentalDuration = EnumRentalDuration.hour12, BasePrice = 610000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanB, RentalDuration = EnumRentalDuration.hour24, BasePrice = 760000m, IsActive = true },

                // ===== SEDAN C (ước tính, vì ảnh không có sedan C rõ ràng) =====
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanC, RentalDuration = EnumRentalDuration.hour4,  BasePrice = 520000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanC, RentalDuration = EnumRentalDuration.hour8,  BasePrice = 660000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanC, RentalDuration = EnumRentalDuration.hour12, BasePrice = 760000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SedanC, RentalDuration = EnumRentalDuration.hour24, BasePrice = 980000m, IsActive = true },

                // ===== SUV B (ước tính, vì ảnh không có SUV B rõ ràng) =====
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvB, RentalDuration = EnumRentalDuration.hour4,  BasePrice = 480000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvB, RentalDuration = EnumRentalDuration.hour8,  BasePrice = 620000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvB, RentalDuration = EnumRentalDuration.hour12, BasePrice = 700000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvB, RentalDuration = EnumRentalDuration.hour24, BasePrice = 900000m, IsActive = true },

                // ===== SUV C (trung bình từ: Corolla Cross + VF e34 trong ảnh) =====
                // 4h: (550+450)/2 = 500.000
                // 8h: (770+600)/2 = 685.000 -> làm tròn 690.000
                // 12h:(880+680)/2 = 780.000
                // 24h:(1100+850)/2 = 975.000 -> làm tròn 980.000
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvC, RentalDuration = EnumRentalDuration.hour4,  BasePrice = 500000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvC, RentalDuration = EnumRentalDuration.hour8,  BasePrice = 690000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvC, RentalDuration = EnumRentalDuration.hour12, BasePrice = 780000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.SuvC, RentalDuration = EnumRentalDuration.hour24, BasePrice = 980000m, IsActive = true },

                // ===== SUV 7 (đúng theo: Honda CR-V 2023 trong ảnh) =====
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Suv7, RentalDuration = EnumRentalDuration.hour4,  BasePrice = 650000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Suv7, RentalDuration = EnumRentalDuration.hour8,  BasePrice = 910000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Suv7, RentalDuration = EnumRentalDuration.hour12, BasePrice = 1040000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Suv7, RentalDuration = EnumRentalDuration.hour24, BasePrice = 1300000m, IsActive = true },

                // ===== MPV 7 (đúng theo: Kia Carnival 2024 trong ảnh) =====
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mpv7, RentalDuration = EnumRentalDuration.hour4,  BasePrice = 1050000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mpv7, RentalDuration = EnumRentalDuration.hour8,  BasePrice = 1470000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mpv7, RentalDuration = EnumRentalDuration.hour12, BasePrice = 1680000m, IsActive = true },
                new() { Id = Guid.NewGuid(), CarType = EnumCarType.Mpv7, RentalDuration = EnumRentalDuration.hour24, BasePrice = 2100000m, IsActive = true },
            };

            await dbContext.Set<BasePrices>().AddRangeAsync(items);
            await dbContext.SaveChangesAsync();
        }
    }
}
