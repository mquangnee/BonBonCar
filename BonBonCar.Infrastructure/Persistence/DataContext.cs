using BonBonCar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<RentalContract> RentalContracts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<VerificationSession> VerificationSessions { get; set; }
        public DbSet<VerificationLog> VerificationLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RegisterOtpSession> RegisterOtpSessions { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<CarPrice> CarPrices { get; set; }
        public DbSet<BasePrices> BasePrices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.ApplyConfigurationsFromAssembly(
            //    typeof(DataContext).Assembly
            //);

            builder.Entity<CarImage>()
                 .HasIndex(x => new { x.VehicleId, x.IsPrimary })
                 .IsUnique()
                 .HasFilter("[IsPrimary] = 1");
        }
    }
}
