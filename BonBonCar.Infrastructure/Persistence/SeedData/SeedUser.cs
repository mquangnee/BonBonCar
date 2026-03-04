using BonBonCar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Persistence.SeedData
{
    public static class SeedUsers
    {
        public static async Task SeedAsync(
            DataContext dbContext,
            UserManager<User> userManager)
        {
            if (await dbContext.Users.AnyAsync())
                return;

            var passwordHasher = new PasswordHasher<User>();

            var rawUsers = new (string FullName, string Email)[]
            {
                ("Nguyen Minh Anh", "minh.anh@bonboncar.com"),
                ("Tran Hoang Nam", "hoang.nam@bonboncar.com"),
                ("Le Thu Trang", "thu.trang@bonboncar.com"),
                ("Pham Quang Huy", "quang.huy@bonboncar.com"),
                ("Vo Thanh Dat", "thanh.dat@bonboncar.com"),
                ("Dang Ngoc Linh", "ngoc.linh@bonboncar.com"),
                ("Bui Gia Bao", "gia.bao@bonboncar.com"),
                ("Doan My Tien", "my.tien@bonboncar.com"),
                ("Phan Duc Tai", "duc.tai@bonboncar.com"),
                ("Hoang Khanh Vy", "khanh.vy@bonboncar.com"),
                ("Ngo Tuan Kiet", "tuan.kiet@bonboncar.com"),
                ("Ly Bao Chau", "bao.chau@bonboncar.com"),
                ("Truong Minh Khoa", "minh.khoa@bonboncar.com"),
                ("Dinh Phuong Thao", "phuong.thao@bonboncar.com"),
                ("Huynh Gia Han", "gia.han@bonboncar.com"),
                ("Mai Thanh Tung", "thanh.tung@bonboncar.com"),
                ("Cao Thuy Duong", "thuy.duong@bonboncar.com"),
                ("Lam Duc Anh", "duc.anh@bonboncar.com"),
                ("Nguyen Ha Linh", "ha.linh@bonboncar.com"),
                ("Tran Quoc Bao", "quoc.bao@bonboncar.com"),
                ("Le Ngoc Mai", "ngoc.mai@bonboncar.com"),
                ("Pham Thanh Nhan", "thanh.nhan@bonboncar.com"),
                ("Vo Minh Tri", "minh.tri@bonboncar.com"),
                ("Dang Kim Oanh", "kim.oanh@bonboncar.com"),
                ("Bui Thanh Son", "thanh.son@bonboncar.com"),
                ("Doan Ngoc Hieu", "ngoc.hieu@bonboncar.com"),
                ("Phan Gia Linh", "gia.linh@bonboncar.com"),
                ("Hoang Anh Tuan", "anh.tuan@bonboncar.com"),
                ("Ngo Thanh Phuc", "thanh.phuc@bonboncar.com"),
                ("Ly Minh Chau", "minh.chau@bonboncar.com")
            };

            foreach (var item in rawUsers)
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = item.Email,
                    NormalizedUserName = item.Email.ToUpper(),
                    Email = item.Email,
                    NormalizedEmail = item.Email.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                };
                user.PasswordHash = passwordHasher.HashPassword(user, "Abcd@123");
                await userManager.CreateAsync(user);
                await userManager.AddToRoleAsync(user, "User");
            }
            await dbContext.SaveChangesAsync();
        }
    }
}