using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CANDIDATES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MEDICAL_ALLOWANCE",
                schema: "HR_RECRUITMENT",
                table: "OFFERS",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OTHER_ALLOWANCES",
                schema: "HR_RECRUITMENT",
                table: "OFFERS",
                type: "decimal(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MEDICAL_ALLOWANCE",
                schema: "HR_RECRUITMENT",
                table: "OFFERS");

            migrationBuilder.DropColumn(
                name: "OTHER_ALLOWANCES",
                schema: "HR_RECRUITMENT",
                table: "OFFERS");
        }
    }
}
