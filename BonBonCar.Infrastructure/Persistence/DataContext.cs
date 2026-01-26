using BonBonCar.Domain.Entities;
using BonBonCar.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Persistence
{
    public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<RentalOrder> RentalOrders { get; set; }
        public DbSet<RentalContract> RentalContracts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<VerificationSession> VerificationSessions { get; set; }
        public DbSet<VerificationLog> VerificationLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Users / Roles
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");

            // Relationship tables
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            //builder.ApplyConfigurationsFromAssembly(
            //    typeof(DataContext).Assembly
            //);

            builder.Entity<VehicleImage>()
                 .HasIndex(x => new { x.VehicleId, x.IsPrimary })
                 .IsUnique()
                 .HasFilter("[IsPrimary] = 1");
        }
    }
}
