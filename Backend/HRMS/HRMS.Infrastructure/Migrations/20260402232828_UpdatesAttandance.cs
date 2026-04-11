using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesAttandance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DEDUCTION_AMOUNT",
                schema: "HR_ATTENDANCE",
                table: "DAILY_ATTENDANCE",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OVERTIME_AMOUNT",
                schema: "HR_ATTENDANCE",
                table: "DAILY_ATTENDANCE",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DEDUCTION_AMOUNT",
                schema: "HR_ATTENDANCE",
                table: "DAILY_ATTENDANCE");

            migrationBuilder.DropColumn(
                name: "OVERTIME_AMOUNT",
                schema: "HR_ATTENDANCE",
                table: "DAILY_ATTENDANCE");
        }
    }
}
