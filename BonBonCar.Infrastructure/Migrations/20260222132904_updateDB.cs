using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonBonCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "Payments",
                newName: "Provider");

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
                name: "UpdatedAt",
                table: "RentalOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProviderResponseCode",
                table: "Payments",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderTransactionNo",
                table: "Payments",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawIpn",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxnRef",
                table: "Payments",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TxnRef",
                table: "Payments",
                column: "TxnRef",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_TxnRef",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "HoldExpiresAt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ProviderResponseCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ProviderTransactionNo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RawIpn",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TxnRef",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "Payments",
                newName: "PaymentType");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
