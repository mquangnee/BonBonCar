using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonBonCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixrentaltable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PickupDateTime",
                table: "RentalOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDateTime",
                table: "RentalOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupDateTime",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ReturnDateTime",
                table: "RentalOrders");
        }
    }
}
