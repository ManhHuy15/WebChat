using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SevenDB_OTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Otp",
                table: "Users",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpires",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Otp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpExpires",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
