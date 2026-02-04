using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class bank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ABSENCE_DAYS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OT_EARNINGS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TOTAL_LATE_MINUTES",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TOTAL_OT_MINUTES",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ABSENCE_DAYS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS");

            migrationBuilder.DropColumn(
                name: "OT_EARNINGS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS");

            migrationBuilder.DropColumn(
                name: "TOTAL_LATE_MINUTES",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS");

            migrationBuilder.DropColumn(
                name: "TOTAL_OT_MINUTES",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS");
        }
    }
}
