using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums;
using BonBonCar.Domain.Enums.Car;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Persistence.SeedData
{
    public static class SeedCars
    {
        public static async Task SeedAsync(DataContext context)
        {
            var ownerId = Guid.Parse("56F2E65F-FBDA-4140-4519-08DE712EBA42");

            if (await context.Cars.AnyAsync(x => x.UserId == ownerId))
                return;

            var cars = new List<Car>
            {
                Create("11111111-1111-1111-1111-111111111001", ownerId, "1B42C0C9-5F8A-47CE-8FEF-1091A8241A82", "1", 2015, "29B199299", "20 Lạc Long Quân, Tây Hồ, Hà Nội",
                    new[] {"cruise","abs","traction","blind_spot","camera360","sunroof"}),

                Create("11111111-1111-1111-1111-111111111002", ownerId, "2048C66A-E2A1-4F4A-A7CF-3D4CDC727A3F", "1", 2024, "30C192145", "2B Quang Trung, Hoàn Kiếm, Hà Nội",
                    new[] {"gps","ac","bluetooth","automatic","heated_seat"}),

                Create("11111111-1111-1111-1111-111111111003", ownerId, "CE4F67A2-FFEE-43F4-A224-AB7C60765C66", "1", 2018, "29B102947", "20 Hai Bà Trưng, Hoàn Kiếm, Hà Nội",
                    new[] {"gps","camera360","sunroof","child_seat"}),

                Create("11111111-1111-1111-1111-111111111004", ownerId, "2AC3E9E5-62E8-4126-AD91-E2463D6B890A", "1", 2018, "29C209234", "Ngõ Hàm Rồng, Bồ Đề, Hà Nội",
                    new[] {"abs","blind_spot","keyless"}),

                Create("11111111-1111-1111-1111-111111111005", ownerId, "6E90D483-BCCF-4DAB-B905-1FF169D0645D", "24", 2020, "99B198043", "12A Bố Hạ, Bắc Ninh",
                    new[] {"cruise","lane_assist","auto_brake"}),

                Create("11111111-1111-1111-1111-111111111006", ownerId, "89390C5D-CBF0-44C6-BDA8-C30CE6B8EA0A", "1", 2025, "29B299120", "18 Lý Thường Kiệt, Hà Nội",
                    new[] {"electric_seat","memory_seat","dash_cam"}),

                Create("11111111-1111-1111-1111-111111111007", ownerId, "FCD24CCC-E020-47E8-B6BA-0A8A88F4812D", "48", 2022, "43A123456", "90 Võ Nguyên Giáp, Đà Nẵng",
                    new[] {"gps","camera360","sunroof","carplay"}),

                Create("11111111-1111-1111-1111-111111111008", ownerId, "4ACC02D0-299D-4FF5-9132-C4C2C6313BBA", "79", 2021, "51H888888", "101 Điện Biên Phủ, TP.HCM",
                    new[] {"ac","automatic","bluetooth"}),

                Create("11111111-1111-1111-1111-111111111009", ownerId, "D0984214-B55E-40ED-AAEA-CB2C511099DF", "1", 2019, "29A567890", "30 Cầu Giấy, Hà Nội",
                    new[] {"abs","traction","push_start"}),

                Create("11111111-1111-1111-1111-111111111010", ownerId, "7CCE5474-BBF4-48C5-81B7-78876C481FCC", "79", 2023, "50G234567", "Nguyễn Văn Cừ, TP.HCM",
                    new[] {"gps","child_seat","camera360"}),

                Create("11111111-1111-1111-1111-111111111011", ownerId, "9950A86B-0BE4-4E4D-8EA9-CACE445E3EAC", "48", 2020, "43B222222", "Hải Châu, Đà Nẵng",
                    new[] {"sunroof","automatic","heated_seat"}),

                Create("11111111-1111-1111-1111-111111111012", ownerId, "36A45A8C-67A3-45C6-8E45-AFA235DBF6BA", "24", 2022, "99C333333", "Từ Sơn, Bắc Ninh",
                    new[] {"lane_assist","forward_collision"}),

                Create("11111111-1111-1111-1111-111111111013", ownerId, "EB5BD918-6C6B-4089-8284-79436112EDC2", "1", 2023, "29D444444", "Ba Đình, Hà Nội",
                    new[] {"carplay","android_auto"}),

                Create("11111111-1111-1111-1111-111111111014", ownerId, "9CBB250A-B9A4-4986-ACC8-C5F23AC1A468", "1", 2021, "29E555555", "Thanh Xuân, Hà Nội",
                    new[] {"abs","gps","camera360"}),

                Create("11111111-1111-1111-1111-111111111015", ownerId, "16FFDBDA-DBA6-41DB-A7F8-5C83C6AEC2DD", "79", 2022, "51F666666", "Quận 7, TP.HCM",
                    new[] {"sunroof","heated_seat"}),

                Create("11111111-1111-1111-1111-111111111016", ownerId, "E913BDEC-E884-4EDC-A5D4-1915B2754498", "1", 2020, "29G777777", "Long Biên, Hà Nội",
                    new[] {"push_start","keyless"}),

                Create("11111111-1111-1111-1111-111111111017", ownerId, "CE4F67A2-FFEE-43F4-A224-AB7C60765C66", "48", 2019, "43H888888", "Sơn Trà, Đà Nẵng",
                    new[] {"automatic","bluetooth"}),

                Create("11111111-1111-1111-1111-111111111018", ownerId, "6E90D483-BCCF-4DAB-B905-1FF169D0645D", "24", 2024, "99K999999", "Yên Phong, Bắc Ninh",
                    new[] {"cruise","lane_assist"}),

                Create("11111111-1111-1111-1111-111111111019", ownerId, "2048C66A-E2A1-4F4A-A7CF-3D4CDC727A3F", "1", 2023, "29L121212", "Đống Đa, Hà Nội",
                    new[] {"gps","camera360","heated_seat"}),

                Create("11111111-1111-1111-1111-111111111020", ownerId, "89390C5D-CBF0-44C6-BDA8-C30CE6B8EA0A", "79", 2025, "51M343434", "Thủ Đức, TP.HCM",
                    new[] {"electric_seat","memory_seat","dash_cam"}),
            };

            await context.Cars.AddRangeAsync(cars);
            await context.SaveChangesAsync();
        }

        private static Car Create(
            string id,
            Guid ownerId,
            string modelId,
            string provinceCode,
            int year,
            string plate,
            string address,
            IEnumerable<string> features)
        {
            return new Car
            {
                Id = Guid.Parse(id),
                UserId = ownerId,
                ModelId = Guid.Parse(modelId),
            };
        }
    }
}