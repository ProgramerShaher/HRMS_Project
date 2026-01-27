using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updatedd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CONTRACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "CONTRACTS");

            migrationBuilder.DropForeignKey(
                name: "FK_DEPENDENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "DEPENDENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMERGENCY_CONTACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMERGENCY_CONTACTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_ADDRESSES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_ADDRESSES");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_BANK_ACCOUNTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_BANK_ACCOUNTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_CERTIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_CERTIFICATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_COMPENSATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_COMPENSATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_EXPERIENCES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_EXPERIENCES");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_QUALIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_QUALIFICATIONS");

            migrationBuilder.AddForeignKey(
                name: "FK_CONTRACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "CONTRACTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DEPENDENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "DEPENDENTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMERGENCY_CONTACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMERGENCY_CONTACTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_ADDRESSES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_ADDRESSES",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_BANK_ACCOUNTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_BANK_ACCOUNTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_CERTIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_CERTIFICATIONS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_COMPENSATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_COMPENSATIONS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_EXPERIENCES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_EXPERIENCES",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_QUALIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_QUALIFICATIONS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CONTRACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "CONTRACTS");

            migrationBuilder.DropForeignKey(
                name: "FK_DEPENDENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "DEPENDENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMERGENCY_CONTACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMERGENCY_CONTACTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_ADDRESSES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_ADDRESSES");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_BANK_ACCOUNTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_BANK_ACCOUNTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_CERTIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_CERTIFICATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_COMPENSATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_COMPENSATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_EXPERIENCES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_EXPERIENCES");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_QUALIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_QUALIFICATIONS");

            migrationBuilder.AddForeignKey(
                name: "FK_CONTRACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "CONTRACTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DEPENDENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "DEPENDENTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMERGENCY_CONTACTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMERGENCY_CONTACTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_ADDRESSES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_ADDRESSES",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_BANK_ACCOUNTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_BANK_ACCOUNTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_CERTIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_CERTIFICATIONS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_COMPENSATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_COMPENSATIONS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_EXPERIENCES_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_EXPERIENCES",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_QUALIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_QUALIFICATIONS",
                column: "EMPLOYEE_ID",
                principalSchema: "HR_PERSONNEL",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
