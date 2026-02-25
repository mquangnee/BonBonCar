using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonBonCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRentalOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalOrders_Cars_CarId",
                table: "RentalOrders");

            migrationBuilder.DropIndex(
                name: "IX_RentalOrders_CarId",
                table: "RentalOrders");

            migrationBuilder.DropIndex(
                name: "IX_RentalContracts_RentalOrderId",
                table: "RentalContracts");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "HoldExpiresAt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RentalOrders");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "RentalOrders",
                newName: "DepositAmount");

            migrationBuilder.RenameColumn(
                name: "RenterId",
                table: "RentalOrders",
                newName: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalContracts_RentalOrderId",
                table: "RentalContracts",
                column: "RentalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RentalContracts_RentalOrderId",
                table: "RentalContracts");

            migrationBuilder.RenameColumn(
                name: "DepositAmount",
                table: "RentalOrders",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "RentalOrders",
                newName: "RenterId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "RentalOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                table: "RentalOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "RentalOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "HoldExpiresAt",
                table: "RentalOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "RentalOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "RentalOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RentalOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_CarId",
                table: "RentalOrders",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalContracts_RentalOrderId",
                table: "RentalContracts",
                column: "RentalOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RentalOrders_Cars_CarId",
                table: "RentalOrders",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
