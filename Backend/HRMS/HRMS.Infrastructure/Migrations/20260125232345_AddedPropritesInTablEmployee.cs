using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPropritesInTablEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_DOCUMENT_TYPES_DOC_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEES_DEPARTMENTS_DEPT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "STATUS",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "ATTACHMENT_PATH",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.RenameColumn(
                name: "JOINING_DATE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "HIRE_DATE");

            migrationBuilder.RenameColumn(
                name: "HIJRI_LAST_NAME_AR",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "LAST_NAME_AR");

            migrationBuilder.RenameColumn(
                name: "DEPT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "DEPARTMENT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_EMPLOYEES_DEPT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "IX_EMPLOYEES_DEPARTMENT_ID");

            migrationBuilder.RenameColumn(
                name: "ISSUE_PLACE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "CONTENT_TYPE");

            migrationBuilder.RenameColumn(
                name: "DOC_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "DOCUMENT_TYPE_ID");

            migrationBuilder.RenameColumn(
                name: "DOC_NUMBER",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "DOCUMENT_NUMBER");

            migrationBuilder.RenameColumn(
                name: "DOC_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "DOCUMENT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_EMPLOYEE_DOCUMENTS_DOC_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "IX_EMPLOYEE_DOCUMENTS_DOCUMENT_TYPE_ID");

            migrationBuilder.AlterColumn<string>(
                name: "THIRD_NAME_AR",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SECOND_NAME_AR",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NATIONALITY_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MOBILE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MARITAL_STATUS",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GENDER",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EMAIL",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LICENSE_EXPIRY_DATE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LICENSE_NUMBER",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NATIONAL_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SPECIALTY",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USER_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FILE_NAME",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FILE_PATH",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FILE_SIZE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CITIZENSHIP_NAME_EN",
                schema: "HR_CORE",
                table: "COUNTRIES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_COMPENSATIONS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    COMPENSATION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    BASIC_SALARY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HOUSING_ALLOWANCE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TRANSPORT_ALLOWANCE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MEDICAL_ALLOWANCE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OTHER_ALLOWANCES = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BANK_ID = table.Column<int>(type: "int", nullable: true),
                    IBAN_NUMBER = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_COMPENSATIONS", x => x.COMPENSATION_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_COMPENSATIONS_BANKS_BANK_ID",
                        column: x => x.BANK_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "BANKS",
                        principalColumn: "BANK_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_COMPENSATIONS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_COMPENSATIONS_BANK_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_COMPENSATIONS",
                column: "BANK_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_COMPENSATIONS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_COMPENSATIONS",
                column: "EMPLOYEE_ID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_DOCUMENT_TYPES_DOCUMENT_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                column: "DOCUMENT_TYPE_ID",
                principalSchema: "HR_CORE",
                principalTable: "DOCUMENT_TYPES",
                principalColumn: "DOCUMENT_TYPE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEES_DEPARTMENTS_DEPARTMENT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                column: "DEPARTMENT_ID",
                principalSchema: "HR_CORE",
                principalTable: "DEPARTMENTS",
                principalColumn: "DEPT_ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_DOCUMENT_TYPES_DOCUMENT_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_EMPLOYEES_DEPARTMENTS_DEPARTMENT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_COMPENSATIONS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LICENSE_EXPIRY_DATE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "LICENSE_NUMBER",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "NATIONAL_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "SPECIALTY",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "USER_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES");

            migrationBuilder.DropColumn(
                name: "FILE_NAME",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropColumn(
                name: "FILE_PATH",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropColumn(
                name: "FILE_SIZE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS");

            migrationBuilder.DropColumn(
                name: "CITIZENSHIP_NAME_EN",
                schema: "HR_CORE",
                table: "COUNTRIES");

            migrationBuilder.RenameColumn(
                name: "LAST_NAME_AR",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "HIJRI_LAST_NAME_AR");

            migrationBuilder.RenameColumn(
                name: "HIRE_DATE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "JOINING_DATE");

            migrationBuilder.RenameColumn(
                name: "DEPARTMENT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "DEPT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_EMPLOYEES_DEPARTMENT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                newName: "IX_EMPLOYEES_DEPT_ID");

            migrationBuilder.RenameColumn(
                name: "DOCUMENT_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "DOC_TYPE_ID");

            migrationBuilder.RenameColumn(
                name: "DOCUMENT_NUMBER",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "DOC_NUMBER");

            migrationBuilder.RenameColumn(
                name: "CONTENT_TYPE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "ISSUE_PLACE");

            migrationBuilder.RenameColumn(
                name: "DOCUMENT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "DOC_ID");

            migrationBuilder.RenameIndex(
                name: "IX_EMPLOYEE_DOCUMENTS_DOCUMENT_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                newName: "IX_EMPLOYEE_DOCUMENTS_DOC_TYPE_ID");

            migrationBuilder.AlterColumn<string>(
                name: "THIRD_NAME_AR",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SECOND_NAME_AR",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "NATIONALITY_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MOBILE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "MARITAL_STATUS",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "GENDER",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1);

            migrationBuilder.AlterColumn<string>(
                name: "EMAIL",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "STATUS",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ATTACHMENT_PATH",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "IS_ACTIVE",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEE_DOCUMENTS_DOCUMENT_TYPES_DOC_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                column: "DOC_TYPE_ID",
                principalSchema: "HR_CORE",
                principalTable: "DOCUMENT_TYPES",
                principalColumn: "DOCUMENT_TYPE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EMPLOYEES_DEPARTMENTS_DEPT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                column: "DEPT_ID",
                principalSchema: "HR_CORE",
                principalTable: "DEPARTMENTS",
                principalColumn: "DEPT_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
