using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CB_WebApiTask.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationfornewSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    IcNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PinHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOtpVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPrivacyAccepted = table.Column<bool>(type: "bit", nullable: false),
                    IsFullyRegistered = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.IcNumber);
                });

            migrationBuilder.CreateTable(
                name: "OtpSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IcNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmailAddress",
                table: "Customers",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_MobileNumber",
                table: "Customers",
                column: "MobileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtpSessions_IcNumber_Purpose_IsVerified",
                table: "OtpSessions",
                columns: new[] { "IcNumber", "Purpose", "IsVerified" });

            migrationBuilder.CreateIndex(
                name: "IX_OtpSessions_MobileNumber_Purpose_IsVerified",
                table: "OtpSessions",
                columns: new[] { "MobileNumber", "Purpose", "IsVerified" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "OtpSessions");
        }
    }
}
