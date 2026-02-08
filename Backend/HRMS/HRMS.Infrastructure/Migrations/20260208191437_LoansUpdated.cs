using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LoansUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "APPROVED_BY",
                schema: "HR_PAYROLL",
                table: "LOANS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                schema: "HR_PAYROLL",
                table: "LOANS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SettlementDate",
                schema: "HR_PAYROLL",
                table: "LOANS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SettlementNotes",
                schema: "HR_PAYROLL",
                table: "LOANS",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                schema: "HR_PAYROLL",
                table: "LOAN_INSTALLMENTS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SettlementNotes",
                schema: "HR_PAYROLL",
                table: "LOAN_INSTALLMENTS",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "HR_PAYROLL",
                table: "LOAN_INSTALLMENTS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APPROVED_BY",
                schema: "HR_PAYROLL",
                table: "LOANS");

            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                schema: "HR_PAYROLL",
                table: "LOANS");

            migrationBuilder.DropColumn(
                name: "SettlementDate",
                schema: "HR_PAYROLL",
                table: "LOANS");

            migrationBuilder.DropColumn(
                name: "SettlementNotes",
                schema: "HR_PAYROLL",
                table: "LOANS");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                schema: "HR_PAYROLL",
                table: "LOAN_INSTALLMENTS");

            migrationBuilder.DropColumn(
                name: "SettlementNotes",
                schema: "HR_PAYROLL",
                table: "LOAN_INSTALLMENTS");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "HR_PAYROLL",
                table: "LOAN_INSTALLMENTS");
        }
    }
}
