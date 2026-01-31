using BonBonCar.Api.Controllers;
using BonBonCar.Application.Commands.AuthCmd;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.IService;
using BonBonCar.Infrastructure.Identity;
using BonBonCar.Infrastructure.Persistence;
using BonBonCar.Infrastructure.Repositories;
using BonBonCar.Infrastructure.Services;
using BonBonCar.Infrastructure.Services.Model;
using BonBonCar.Infrastructure.Services.Sender;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BonBonCar.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
      {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Kết nối SQL Server
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
            });

            // Đăng ký Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            // Yêu cầu về mật khẩu
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true; //Yêu cầu chữ số
                options.Password.RequireLowercase = true; //Yêu cầu chữ thường
                options.Password.RequireNonAlphanumeric = true; //Yêu cầu ký tự đặc biệt
                options.Password.RequireUppercase = true; //Yêu cầu chữ hoa
                options.Password.RequiredLength = 6; //Độ dài tối thiểu
                options.Password.RequiredUniqueChars = 1; //Số ký tự đặc biệt
            });

            // Đăng ký Repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IRentalContractRepository, RentalContractRepository>();
            builder.Services.AddScoped<IRentalOrderRepository, RentalOrderRepository>();
            builder.Services.AddScoped<IUserDocumentRepository, UserDocumentRepository>();
            builder.Services.AddScoped<IVehicleImageRepository, VehicleImageRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IVerificationLogRepository, VerificationLogRepository>();
            builder.Services.AddScoped<IVerificationSessionRepository, VerificationSessionRepository>();
            builder.Services.AddScoped<IRegisterOtpSessionRepository, RegisterOtpSessionRepository>();

            // Đăng ký Service
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddMediatR(typeof(RegisterCmd).Assembly);
            builder.Services.AddScoped<IEmailTemplate, EmailTemplate>();

            // Đăng ký Jwt Bearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwt = builder.Configuration.GetSection("Jwt");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

            // Đăng ký Authorization
            builder.Services.AddAuthorization();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Seed Role
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                await IdentitySeed.SeedRolesAsync(roleManager);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
