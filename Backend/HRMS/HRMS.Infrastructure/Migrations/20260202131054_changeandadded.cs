using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeandadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ATTENDANCE_CORRECTIONS",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    CORRECTION_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    ATTENDANCE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DAILY_ATTENDANCE_ID = table.Column<long>(type: "bigint", nullable: false),
                    FIELD_NAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OLD_VALUE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NEW_VALUE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AUDIT_NOTE = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATTENDANCE_CORRECTIONS", x => x.CORRECTION_ID);
                    table.ForeignKey(
                        name: "FK_ATTENDANCE_CORRECTIONS_DAILY_ATTENDANCE_DAILY_ATTENDANCE_ID",
                        column: x => x.DAILY_ATTENDANCE_ID,
                        principalSchema: "HR_ATTENDANCE",
                        principalTable: "DAILY_ATTENDANCE",
                        principalColumn: "RECORD_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ATTENDANCE_CORRECTIONS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEAVE_APPROVAL_HISTORY",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    HISTORY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    REQUEST_ID = table.Column<int>(type: "int", nullable: false),
                    ACTION_TYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PERFORMED_BY_USER_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PERFORMED_BY_EMPLOYEE_ID = table.Column<int>(type: "int", nullable: true),
                    ACTION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    COMMENT = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PREVIOUS_STATUS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NEW_STATUS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVE_APPROVAL_HISTORY", x => x.HISTORY_ID);
                    table.ForeignKey(
                        name: "FK_LEAVE_APPROVAL_HISTORY_EMPLOYEES_PERFORMED_BY_EMPLOYEE_ID",
                        column: x => x.PERFORMED_BY_EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LEAVE_APPROVAL_HISTORY_LEAVE_REQUESTS_REQUEST_ID",
                        column: x => x.REQUEST_ID,
                        principalSchema: "HR_LEAVES",
                        principalTable: "LEAVE_REQUESTS",
                        principalColumn: "REQUEST_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATTENDANCE_CORRECTIONS_DAILY_ATTENDANCE_ID",
                schema: "HR_ATTENDANCE",
                table: "ATTENDANCE_CORRECTIONS",
                column: "DAILY_ATTENDANCE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ATTENDANCE_CORRECTIONS_EMPLOYEE_ID",
                schema: "HR_ATTENDANCE",
                table: "ATTENDANCE_CORRECTIONS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_APPROVAL_HISTORY_PERFORMED_BY_EMPLOYEE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_APPROVAL_HISTORY",
                column: "PERFORMED_BY_EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_APPROVAL_HISTORY_REQUEST_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_APPROVAL_HISTORY",
                column: "REQUEST_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATTENDANCE_CORRECTIONS",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "LEAVE_APPROVAL_HISTORY",
                schema: "HR_LEAVES");
        }
    }
}
