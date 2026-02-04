using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IS_ACTIVE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PROFILE_PICTURE_PATH",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TERMINATION_DATE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "PROFILE_PICTURE_PATH",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "TERMINATION_DATE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");
        }
    }
}
