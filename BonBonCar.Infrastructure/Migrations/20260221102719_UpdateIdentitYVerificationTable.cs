using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonBonCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentitYVerificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CccdDateOfBirth",
                table: "IdentityVerifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CccdFullName",
                table: "IdentityVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CccdNationality",
                table: "IdentityVerifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CccdPlaceOfResidence",
                table: "IdentityVerifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CccdDateOfBirth",
                table: "IdentityVerifications");

            migrationBuilder.DropColumn(
                name: "CccdFullName",
                table: "IdentityVerifications");

            migrationBuilder.DropColumn(
                name: "CccdNationality",
                table: "IdentityVerifications");

            migrationBuilder.DropColumn(
                name: "CccdPlaceOfResidence",
                table: "IdentityVerifications");
        }
    }
}
