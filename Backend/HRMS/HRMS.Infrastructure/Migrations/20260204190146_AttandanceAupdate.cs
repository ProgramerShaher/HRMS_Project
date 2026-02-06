using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AttandanceAupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PERMISSION_REQUESTS",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    PERMISSION_REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    PERMISSION_TYPE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PERMISSION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HOURS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    REASON = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    REJECTION_REASON = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    APPROVED_BY = table.Column<int>(type: "int", nullable: true),
                    APPROVED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERMISSION_REQUESTS", x => x.PERMISSION_REQUEST_ID);
                    table.ForeignKey(
                        name: "FK_PERMISSION_REQUESTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PERMISSION_REQUESTS_EMPLOYEE_ID",
                schema: "HR_ATTENDANCE",
                table: "PERMISSION_REQUESTS",
                column: "EMPLOYEE_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PERMISSION_REQUESTS",
                schema: "HR_ATTENDANCE");
        }
    }
}
