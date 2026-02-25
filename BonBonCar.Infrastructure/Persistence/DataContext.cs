using BonBonCar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BonBonCar.Infrastructure.Persistence
{
    public class DataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarImage> CarImages { get; set; }
        public DbSet<RentalOrder> RentalOrders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RegisterOtpSession> RegisterOtpSessions { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<CarPrice> CarPrices { get; set; }
        public DbSet<BasePrices> BasePrices { get; set; }
        public DbSet<IdentityVerification> IdentityVerifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CarImage>()
                .HasIndex(x => new { x.CarId, x.IsPrimary })
                .IsUnique()
                .HasFilter("[IsPrimary] = 1");
            builder.Entity<Payment>()
                .HasIndex(x => x.TxnRef)
                .IsUnique();
        }
    }
}
