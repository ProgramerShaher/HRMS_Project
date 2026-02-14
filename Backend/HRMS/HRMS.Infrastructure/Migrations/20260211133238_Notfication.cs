using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Notfication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MESSAGE_AR",
                schema: "HR_CORE",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "NOTIFICATION_TYPE",
                schema: "HR_CORE",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "PRIORITY",
                schema: "HR_CORE",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "READ_AT",
                schema: "HR_CORE",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "RECIPIENT_ID",
                schema: "HR_CORE",
                table: "NOTIFICATIONS");

            migrationBuilder.EnsureSchema(
                name: "HR_COMMON");

            migrationBuilder.RenameTable(
                name: "NOTIFICATIONS",
                schema: "HR_CORE",
                newName: "NOTIFICATIONS",
                newSchema: "HR_COMMON");

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

            migrationBuilder.RenameColumn(
                name: "TITLE_AR",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                newName: "TITLE");

            migrationBuilder.RenameColumn(
                name: "REFERENCE_TABLE",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                newName: "REFERENCE_TYPE");

            migrationBuilder.AddColumn<decimal>(
                name: "OTHER_DEDUCTIONS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TOTAL_VIOLATIONS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "ELEMENT_ID",
                schema: "HR_PAYROLL",
                table: "PAYSLIP_DETAILS",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");



            migrationBuilder.AlterColumn<string>(
                name: "REFERENCE_ID",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IS_READ",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NOTIFICATIONS",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "NOTIFICATION_ID",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS");

            migrationBuilder.AddColumn<Guid>(
                name: "NOTIFICATION_ID",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NOTIFICATIONS",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                column: "NOTIFICATION_ID");

            migrationBuilder.AddColumn<string>(
                name: "MESSAGE",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TYPE",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "USER_ID",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTHER_DEDUCTIONS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS");

            migrationBuilder.DropColumn(
                name: "TOTAL_VIOLATIONS",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS");



            migrationBuilder.DropColumn(
                name: "MESSAGE",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "TYPE",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "USER_ID",
                schema: "HR_COMMON",
                table: "NOTIFICATIONS");

            migrationBuilder.RenameTable(
                name: "NOTIFICATIONS",
                schema: "HR_COMMON",
                newName: "NOTIFICATIONS",
                newSchema: "HR_CORE");

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

            migrationBuilder.RenameColumn(
                name: "TITLE",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                newName: "TITLE_AR");

            migrationBuilder.RenameColumn(
                name: "REFERENCE_TYPE",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                newName: "REFERENCE_TABLE");

            migrationBuilder.AlterColumn<int>(
                name: "ELEMENT_ID",
                schema: "HR_PAYROLL",
                table: "PAYSLIP_DETAILS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "REFERENCE_ID",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "IS_READ",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NOTIFICATIONS",
                schema: "HR_CORE",
                table: "NOTIFICATIONS");

            migrationBuilder.DropColumn(
                name: "NOTIFICATION_ID",
                schema: "HR_CORE",
                table: "NOTIFICATIONS");

            migrationBuilder.AddColumn<long>(
                name: "NOTIFICATION_ID",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NOTIFICATIONS",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                column: "NOTIFICATION_ID");

            migrationBuilder.AddColumn<string>(
                name: "MESSAGE_AR",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOTIFICATION_TYPE",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PRIORITY",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "READ_AT",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RECIPIENT_ID",
                schema: "HR_CORE",
                table: "NOTIFICATIONS",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
