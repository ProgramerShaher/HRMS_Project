using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IS_TAXABLE",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IsTaxable");

            migrationBuilder.RenameColumn(
                name: "IS_RECURRING",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IsRecurring");

            migrationBuilder.RenameColumn(
                name: "IS_GOSI_BASE",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IsGosiBase");

            migrationBuilder.RenameColumn(
                name: "IS_BASIC",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IsBasic");

            migrationBuilder.RenameColumn(
                name: "ELEMENT_TYPE",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "ElementType");

            migrationBuilder.RenameColumn(
                name: "ELEMENT_NAME_AR",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "ElementNameAr");

            migrationBuilder.RenameColumn(
                name: "DEFAULT_PERCENTAGE",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "DefaultPercentage");

            migrationBuilder.RenameColumn(
                name: "ELEMENT_ID",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "ElementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsTaxable",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IS_TAXABLE");

            migrationBuilder.RenameColumn(
                name: "IsRecurring",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IS_RECURRING");

            migrationBuilder.RenameColumn(
                name: "IsGosiBase",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IS_GOSI_BASE");

            migrationBuilder.RenameColumn(
                name: "IsBasic",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "IS_BASIC");

            migrationBuilder.RenameColumn(
                name: "ElementType",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "ELEMENT_TYPE");

            migrationBuilder.RenameColumn(
                name: "ElementNameAr",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "ELEMENT_NAME_AR");

            migrationBuilder.RenameColumn(
                name: "DefaultPercentage",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "DEFAULT_PERCENTAGE");

            migrationBuilder.RenameColumn(
                name: "ElementId",
                schema: "HR_PAYROLL",
                table: "SALARY_ELEMENTS",
                newName: "ELEMENT_ID");
        }
    }
}
