using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonBonCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalRentalFee",
                table: "RentalOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalRentalFee",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Payments");
        }
    }
}
