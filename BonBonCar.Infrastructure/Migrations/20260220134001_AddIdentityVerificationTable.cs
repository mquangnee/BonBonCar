using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonBonCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityVerificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CccdVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlxVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityVerifications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityVerifications");
        }
    }
}
