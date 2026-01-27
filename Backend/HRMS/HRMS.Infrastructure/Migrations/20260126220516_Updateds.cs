using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DEFAULT_DAYS",
                schema: "HR_LEAVES",
                table: "LEAVE_TYPES",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IS_DEDUCTIBLE",
                schema: "HR_LEAVES",
                table: "LEAVE_TYPES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LEAVE_NAME_EN",
                schema: "HR_LEAVES",
                table: "LEAVE_TYPES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DEFAULT_DAYS",
                schema: "HR_LEAVES",
                table: "LEAVE_TYPES");

            migrationBuilder.DropColumn(
                name: "IS_DEDUCTIBLE",
                schema: "HR_LEAVES",
                table: "LEAVE_TYPES");

            migrationBuilder.DropColumn(
                name: "LEAVE_NAME_EN",
                schema: "HR_LEAVES",
                table: "LEAVE_TYPES");
        }
    }
}
