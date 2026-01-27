using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GRADE_NAME",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "TICKET_CLASS",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "ALERT_DAYS_BEFORE",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "IS_MANDATORY",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "REQUIRES_EXPIRY",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.RenameColumn(
                name: "GRADE_ID",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                newName: "JOB_GRADE_ID");

            migrationBuilder.RenameColumn(
                name: "DOC_NAME_AR",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                newName: "DOCUMENT_TYPE_NAME_EN");

            migrationBuilder.RenameColumn(
                name: "DOC_TYPE_ID",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                newName: "DOCUMENT_TYPE_ID");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_SALARY",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_SALARY",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BENEFITS_CONFIG",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DESCRIPTION",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GRADE_CODE",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GRADE_LEVEL",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GRADE_NAME_AR",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GRADE_NAME_EN",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ALLOWED_EXTENSIONS",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DEFAULT_EXPIRY_DAYS",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DESCRIPTION",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DOCUMENT_TYPE_NAME_AR",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HAS_EXPIRY",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IS_REQUIRED",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MAX_FILE_SIZE_MB",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                schema: "HR_CORE",
                table: "DEPARTMENTS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ADDRESS",
                schema: "HR_CORE",
                table: "BANKS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EMAIL",
                schema: "HR_CORE",
                table: "BANKS",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PHONE",
                schema: "HR_CORE",
                table: "BANKS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SWIFT_CODE",
                schema: "HR_CORE",
                table: "BANKS",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BRANCHES",
                schema: "HR_CORE",
                columns: table => new
                {
                    BRANCH_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BRANCH_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BRANCH_NAME_EN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CITY_ID = table.Column<int>(type: "int", nullable: true),
                    ADDRESS = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRANCHES", x => x.BRANCH_ID);
                    table.ForeignKey(
                        name: "FK_BRANCHES_CITIES_CITY_ID",
                        column: x => x.CITY_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "CITIES",
                        principalColumn: "CITY_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DEPARTMENTS_BranchId",
                schema: "HR_CORE",
                table: "DEPARTMENTS",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BRANCHES_CITY_ID",
                schema: "HR_CORE",
                table: "BRANCHES",
                column: "CITY_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DEPARTMENTS_BRANCHES_BranchId",
                schema: "HR_CORE",
                table: "DEPARTMENTS",
                column: "BranchId",
                principalSchema: "HR_CORE",
                principalTable: "BRANCHES",
                principalColumn: "BRANCH_ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DEPARTMENTS_BRANCHES_BranchId",
                schema: "HR_CORE",
                table: "DEPARTMENTS");

            migrationBuilder.DropTable(
                name: "BRANCHES",
                schema: "HR_CORE");

            migrationBuilder.DropIndex(
                name: "IX_DEPARTMENTS_BranchId",
                schema: "HR_CORE",
                table: "DEPARTMENTS");

            migrationBuilder.DropColumn(
                name: "BENEFITS_CONFIG",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "DESCRIPTION",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "GRADE_CODE",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "GRADE_LEVEL",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "GRADE_NAME_AR",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "GRADE_NAME_EN",
                schema: "HR_CORE",
                table: "JOB_GRADES");

            migrationBuilder.DropColumn(
                name: "ALLOWED_EXTENSIONS",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "DEFAULT_EXPIRY_DAYS",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "DESCRIPTION",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "DOCUMENT_TYPE_NAME_AR",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "HAS_EXPIRY",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "IS_REQUIRED",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "MAX_FILE_SIZE_MB",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "HR_CORE",
                table: "DEPARTMENTS");

            migrationBuilder.DropColumn(
                name: "ADDRESS",
                schema: "HR_CORE",
                table: "BANKS");

            migrationBuilder.DropColumn(
                name: "EMAIL",
                schema: "HR_CORE",
                table: "BANKS");

            migrationBuilder.DropColumn(
                name: "PHONE",
                schema: "HR_CORE",
                table: "BANKS");

            migrationBuilder.DropColumn(
                name: "SWIFT_CODE",
                schema: "HR_CORE",
                table: "BANKS");

            migrationBuilder.RenameColumn(
                name: "JOB_GRADE_ID",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                newName: "GRADE_ID");

            migrationBuilder.RenameColumn(
                name: "DOCUMENT_TYPE_NAME_EN",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                newName: "DOC_NAME_AR");

            migrationBuilder.RenameColumn(
                name: "DOCUMENT_TYPE_ID",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                newName: "DOC_TYPE_ID");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_SALARY",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_SALARY",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "GRADE_NAME",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TICKET_CLASS",
                schema: "HR_CORE",
                table: "JOB_GRADES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ALERT_DAYS_BEFORE",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "IS_MANDATORY",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "REQUIRES_EXPIRY",
                schema: "HR_CORE",
                table: "DOCUMENT_TYPES",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
