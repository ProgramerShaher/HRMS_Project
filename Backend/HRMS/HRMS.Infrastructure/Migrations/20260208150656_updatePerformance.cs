using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatePerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FEEDBACK",
                schema: "HR_RECRUITMENT",
                table: "INTERVIEWS",
                newName: "RESULT_NOTES");

            migrationBuilder.RenameColumn(
                name: "SCORE",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS",
                newName: "MANAGER_SCORE");

            migrationBuilder.AddColumn<DateTime>(
                name: "EXPIRY_DATE",
                schema: "HR_RECRUITMENT",
                table: "OFFERS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WEIGHT",
                schema: "HR_PERFORMANCE",
                table: "KPI_LIBRARIES",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "STATUS",
                schema: "HR_RECRUITMENT",
                table: "INTERVIEWS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "COMMENTS",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_APPRAISALS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EMPLOYEE_SCORE",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FINAL_SCORE",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EXPIRY_DATE",
                schema: "HR_RECRUITMENT",
                table: "OFFERS");

            migrationBuilder.DropColumn(
                name: "WEIGHT",
                schema: "HR_PERFORMANCE",
                table: "KPI_LIBRARIES");

            migrationBuilder.DropColumn(
                name: "STATUS",
                schema: "HR_RECRUITMENT",
                table: "INTERVIEWS");

            migrationBuilder.DropColumn(
                name: "COMMENTS",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_APPRAISALS");

            migrationBuilder.DropColumn(
                name: "EMPLOYEE_SCORE",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS");

            migrationBuilder.DropColumn(
                name: "FINAL_SCORE",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS");

            migrationBuilder.RenameColumn(
                name: "RESULT_NOTES",
                schema: "HR_RECRUITMENT",
                table: "INTERVIEWS",
                newName: "FEEDBACK");

            migrationBuilder.RenameColumn(
                name: "MANAGER_SCORE",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS",
                newName: "SCORE");
        }
    }
}
