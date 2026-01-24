using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Intial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "HR_RECRUITMENT");

            migrationBuilder.EnsureSchema(
                name: "HR_PERFORMANCE");

            migrationBuilder.EnsureSchema(
                name: "HR_ATTENDANCE");

            migrationBuilder.EnsureSchema(
                name: "HR_CORE");

            migrationBuilder.EnsureSchema(
                name: "HR_PERSONNEL");

            migrationBuilder.EnsureSchema(
                name: "HR_LEAVES");

            migrationBuilder.EnsureSchema(
                name: "HR_PAYROLL");

            migrationBuilder.CreateTable(
                name: "APPRAISAL_CYCLES",
                schema: "HR_PERFORMANCE",
                columns: table => new
                {
                    CYCLE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CYCLE_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPRAISAL_CYCLES", x => x.CYCLE_ID);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_LOGS",
                schema: "HR_CORE",
                columns: table => new
                {
                    LOG_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TABLE_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RECORD_ID = table.Column<long>(type: "bigint", nullable: false),
                    ACTION_TYPE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OLD_VALUE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NEW_VALUE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PERFORMED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PERFORMED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IP_ADDRESS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    USER_AGENT = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_LOGS", x => x.LOG_ID);
                });

            migrationBuilder.CreateTable(
                name: "BANKS",
                schema: "HR_CORE",
                columns: table => new
                {
                    BANK_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BANK_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BANK_NAME_EN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BANK_CODE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BANKS", x => x.BANK_ID);
                });

            migrationBuilder.CreateTable(
                name: "COUNTRIES",
                schema: "HR_CORE",
                columns: table => new
                {
                    COUNTRY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    COUNTRY_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    COUNTRY_NAME_EN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CITIZENSHIP_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ISO_CODE = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRIES", x => x.COUNTRY_ID);
                });

            migrationBuilder.CreateTable(
                name: "DEPARTMENTS",
                schema: "HR_CORE",
                columns: table => new
                {
                    DEPT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DEPT_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DEPT_NAME_EN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PARENT_DEPT_ID = table.Column<int>(type: "int", nullable: true),
                    COST_CENTER_CODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MANAGER_ID = table.Column<int>(type: "int", nullable: true),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEPARTMENTS", x => x.DEPT_ID);
                    table.ForeignKey(
                        name: "FK_DEPARTMENTS_DEPARTMENTS_PARENT_DEPT_ID",
                        column: x => x.PARENT_DEPT_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "DEPARTMENTS",
                        principalColumn: "DEPT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DISCIPLINARY_ACTIONS",
                schema: "HR_PERFORMANCE",
                columns: table => new
                {
                    ACTION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACTION_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DEDUCTION_DAYS = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    IS_TERMINATION = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISCIPLINARY_ACTIONS", x => x.ACTION_ID);
                });

            migrationBuilder.CreateTable(
                name: "DOCUMENT_TYPES",
                schema: "HR_CORE",
                columns: table => new
                {
                    DOC_TYPE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOC_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IS_MANDATORY = table.Column<byte>(type: "tinyint", nullable: false),
                    REQUIRES_EXPIRY = table.Column<byte>(type: "tinyint", nullable: false),
                    ALERT_DAYS_BEFORE = table.Column<short>(type: "smallint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENT_TYPES", x => x.DOC_TYPE_ID);
                });

            migrationBuilder.CreateTable(
                name: "JOB_GRADES",
                schema: "HR_CORE",
                columns: table => new
                {
                    GRADE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GRADE_NAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MIN_SALARY = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MAX_SALARY = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TICKET_CLASS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOB_GRADES", x => x.GRADE_ID);
                });

            migrationBuilder.CreateTable(
                name: "KPI_LIBRARIES",
                schema: "HR_PERFORMANCE",
                columns: table => new
                {
                    KPI_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KPI_NAME_AR = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KPI_DESCRIPTION = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CATEGORY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MEASUREMENT_UNIT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPI_LIBRARIES", x => x.KPI_ID);
                });

            migrationBuilder.CreateTable(
                name: "LEAVE_TYPES",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    LEAVE_TYPE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LEAVE_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IS_PAID = table.Column<byte>(type: "tinyint", nullable: false),
                    MAX_DAYS_PER_YEAR = table.Column<short>(type: "smallint", nullable: true),
                    REQUIRES_ATTACHMENT = table.Column<byte>(type: "tinyint", nullable: false),
                    IS_CARRY_FORWARD = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVE_TYPES", x => x.LEAVE_TYPE_ID);
                });

            migrationBuilder.CreateTable(
                name: "NOTIFICATIONS",
                schema: "HR_CORE",
                columns: table => new
                {
                    NOTIFICATION_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RECIPIENT_ID = table.Column<int>(type: "int", nullable: false),
                    NOTIFICATION_TYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TITLE_AR = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MESSAGE_AR = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IS_READ = table.Column<byte>(type: "tinyint", nullable: false),
                    READ_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PRIORITY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    REFERENCE_TABLE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    REFERENCE_ID = table.Column<long>(type: "bigint", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATIONS", x => x.NOTIFICATION_ID);
                });

            migrationBuilder.CreateTable(
                name: "PAYROLL_RUNS",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    RunId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Month = table.Column<byte>(type: "tinyint", nullable: false),
                    RunDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TotalPayout = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYROLL_RUNS", x => x.RunId);
                });

            migrationBuilder.CreateTable(
                name: "PUBLIC_HOLIDAYS",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    HOLIDAY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HOLIDAY_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YEAR = table.Column<short>(type: "smallint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PUBLIC_HOLIDAYS", x => x.HOLIDAY_ID);
                });

            migrationBuilder.CreateTable(
                name: "REPORT_TEMPLATES",
                schema: "HR_CORE",
                columns: table => new
                {
                    TEMPLATE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TEMPLATE_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    REPORT_TYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SQL_QUERY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PARAMETERS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORT_TEMPLATES", x => x.TEMPLATE_ID);
                });

            migrationBuilder.CreateTable(
                name: "ROSTER_PERIODS",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    PERIOD_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YEAR = table.Column<short>(type: "smallint", nullable: false),
                    MONTH = table.Column<byte>(type: "tinyint", nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_LOCKED = table.Column<byte>(type: "tinyint", nullable: false),
                    LOCKED_BY = table.Column<int>(type: "int", nullable: true),
                    NOTES = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROSTER_PERIODS", x => x.PERIOD_ID);
                });

            migrationBuilder.CreateTable(
                name: "SALARY_ELEMENTS",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    ElementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElementNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ElementType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsTaxable = table.Column<byte>(type: "tinyint", nullable: false),
                    IsGosiBase = table.Column<byte>(type: "tinyint", nullable: false),
                    DefaultPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsRecurring = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALARY_ELEMENTS", x => x.ElementId);
                });

            migrationBuilder.CreateTable(
                name: "SHIFT_TYPES",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    SHIFT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SHIFT_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    START_TIME = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    END_TIME = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    HOURS_COUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IS_CROSS_DAY = table.Column<byte>(type: "tinyint", nullable: false),
                    GRACE_PERIOD_MINS = table.Column<short>(type: "smallint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHIFT_TYPES", x => x.SHIFT_ID);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_SETTINGS",
                schema: "HR_CORE",
                columns: table => new
                {
                    SETTING_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SETTING_KEY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SETTING_VALUE = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SETTING_TYPE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DESCRIPTION_AR = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IS_EDITABLE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_SETTINGS", x => x.SETTING_ID);
                });

            migrationBuilder.CreateTable(
                name: "VIOLATION_TYPES",
                schema: "HR_PERFORMANCE",
                columns: table => new
                {
                    VIOLATION_TYPE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VIOLATION_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SEVERITY_LEVEL = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VIOLATION_TYPES", x => x.VIOLATION_TYPE_ID);
                });

            migrationBuilder.CreateTable(
                name: "WORKFLOW_APPROVALS",
                schema: "HR_CORE",
                columns: table => new
                {
                    APPROVAL_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    REQUEST_TYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    REQUEST_ID = table.Column<int>(type: "int", nullable: false),
                    APPROVER_LEVEL = table.Column<byte>(type: "tinyint", nullable: false),
                    APPROVER_ID = table.Column<int>(type: "int", nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    APPROVAL_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    COMMENTS = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORKFLOW_APPROVALS", x => x.APPROVAL_ID);
                });

            migrationBuilder.CreateTable(
                name: "CANDIDATES",
                schema: "HR_RECRUITMENT",
                columns: table => new
                {
                    CANDIDATE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FIRST_NAME_AR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FAMILY_NAME_AR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FULL_NAME_EN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PHONE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NATIONALITY_ID = table.Column<int>(type: "int", nullable: true),
                    CV_FILE_PATH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LINKEDIN_PROFILE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CANDIDATES", x => x.CANDIDATE_ID);
                    table.ForeignKey(
                        name: "FK_CANDIDATES_COUNTRIES_NATIONALITY_ID",
                        column: x => x.NATIONALITY_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "COUNTRIES",
                        principalColumn: "COUNTRY_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CITIES",
                schema: "HR_CORE",
                columns: table => new
                {
                    CITY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    COUNTRY_ID = table.Column<int>(type: "int", nullable: false),
                    CITY_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CITY_NAME_EN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CITIES", x => x.CITY_ID);
                    table.ForeignKey(
                        name: "FK_CITIES_COUNTRIES_COUNTRY_ID",
                        column: x => x.COUNTRY_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "COUNTRIES",
                        principalColumn: "COUNTRY_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JOBS",
                schema: "HR_CORE",
                columns: table => new
                {
                    JOB_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JOB_TITLE_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JOB_TITLE_EN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DEFAULT_GRADE_ID = table.Column<int>(type: "int", nullable: true),
                    IS_MEDICAL = table.Column<byte>(type: "tinyint", nullable: false),
                    DepartmentDeptId = table.Column<int>(type: "int", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBS", x => x.JOB_ID);
                    table.ForeignKey(
                        name: "FK_JOBS_DEPARTMENTS_DepartmentDeptId",
                        column: x => x.DepartmentDeptId,
                        principalSchema: "HR_CORE",
                        principalTable: "DEPARTMENTS",
                        principalColumn: "DEPT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JOBS_JOB_GRADES_DEFAULT_GRADE_ID",
                        column: x => x.DEFAULT_GRADE_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "JOB_GRADES",
                        principalColumn: "GRADE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEAVE_ACCRUAL_RULES",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    RULE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LEAVE_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    ACCRUAL_FREQUENCY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DAYS_PER_PERIOD = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MAX_ACCUMULATION = table.Column<short>(type: "smallint", nullable: true),
                    IS_PRORATED = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVE_ACCRUAL_RULES", x => x.RULE_ID);
                    table.ForeignKey(
                        name: "FK_LEAVE_ACCRUAL_RULES_LEAVE_TYPES_LEAVE_TYPE_ID",
                        column: x => x.LEAVE_TYPE_ID,
                        principalSchema: "HR_LEAVES",
                        principalTable: "LEAVE_TYPES",
                        principalColumn: "LEAVE_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ATTENDANCE_POLICIES",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    POLICY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POLICY_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DEPT_ID = table.Column<int>(type: "int", nullable: true),
                    JOB_ID = table.Column<int>(type: "int", nullable: true),
                    LATE_GRACE_MINS = table.Column<short>(type: "smallint", nullable: false),
                    OVERTIME_MULTIPLIER = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    WEEKEND_OT_MULTIPLIER = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATTENDANCE_POLICIES", x => x.POLICY_ID);
                    table.ForeignKey(
                        name: "FK_ATTENDANCE_POLICIES_DEPARTMENTS_DEPT_ID",
                        column: x => x.DEPT_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "DEPARTMENTS",
                        principalColumn: "DEPT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ATTENDANCE_POLICIES_JOBS_JOB_ID",
                        column: x => x.JOB_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "JOBS",
                        principalColumn: "JOB_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEES",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_NUMBER = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FIRST_NAME_AR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SECOND_NAME_AR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    THIRD_NAME_AR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HIJRI_LAST_NAME_AR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FULL_NAME_EN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GENDER = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    BIRTH_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MARITAL_STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NATIONALITY_ID = table.Column<int>(type: "int", nullable: false),
                    JOB_ID = table.Column<int>(type: "int", nullable: false),
                    DEPT_ID = table.Column<int>(type: "int", nullable: false),
                    MANAGER_ID = table.Column<int>(type: "int", nullable: true),
                    JOINING_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MOBILE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEES", x => x.EMPLOYEE_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEES_COUNTRIES_NATIONALITY_ID",
                        column: x => x.NATIONALITY_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "COUNTRIES",
                        principalColumn: "COUNTRY_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEES_DEPARTMENTS_DEPT_ID",
                        column: x => x.DEPT_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "DEPARTMENTS",
                        principalColumn: "DEPT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEES_EMPLOYEES_MANAGER_ID",
                        column: x => x.MANAGER_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEES_JOBS_JOB_ID",
                        column: x => x.JOB_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "JOBS",
                        principalColumn: "JOB_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JOB_VACANCIES",
                schema: "HR_RECRUITMENT",
                columns: table => new
                {
                    VACANCY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JOB_ID = table.Column<int>(type: "int", nullable: false),
                    DEPT_ID = table.Column<int>(type: "int", nullable: false),
                    REQUIRED_COUNT = table.Column<short>(type: "smallint", nullable: false),
                    JOB_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    REQUIREMENTS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PUBLISH_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CLOSING_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOB_VACANCIES", x => x.VACANCY_ID);
                    table.ForeignKey(
                        name: "FK_JOB_VACANCIES_DEPARTMENTS_DEPT_ID",
                        column: x => x.DEPT_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "DEPARTMENTS",
                        principalColumn: "DEPT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JOB_VACANCIES_JOBS_JOB_ID",
                        column: x => x.JOB_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "JOBS",
                        principalColumn: "JOB_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONTRACTS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    CONTRACT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    CONTRACT_TYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_RENEWABLE = table.Column<byte>(type: "tinyint", nullable: false),
                    BASIC_SALARY = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    HOUSING_ALLOWANCE = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TRANSPORT_ALLOWANCE = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OTHER_ALLOWANCES = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    VACATION_DAYS = table.Column<short>(type: "smallint", nullable: false),
                    WORKING_HOURS_DAILY = table.Column<byte>(type: "tinyint", nullable: false),
                    CONTRACT_STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRACTS", x => x.CONTRACT_ID);
                    table.ForeignKey(
                        name: "FK_CONTRACTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DAILY_ATTENDANCE",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    RECORD_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    ATTENDANCE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PLANNED_SHIFT_ID = table.Column<int>(type: "int", nullable: true),
                    ACTUAL_IN_TIME = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ACTUAL_OUT_TIME = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LATE_MINUTES = table.Column<short>(type: "smallint", nullable: false),
                    EARLY_LEAVE_MINUTES = table.Column<short>(type: "smallint", nullable: false),
                    OVERTIME_MINUTES = table.Column<short>(type: "smallint", nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAILY_ATTENDANCE", x => x.RECORD_ID);
                    table.ForeignKey(
                        name: "FK_DAILY_ATTENDANCE_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DAILY_ATTENDANCE_SHIFT_TYPES_PLANNED_SHIFT_ID",
                        column: x => x.PLANNED_SHIFT_ID,
                        principalSchema: "HR_ATTENDANCE",
                        principalTable: "SHIFT_TYPES",
                        principalColumn: "SHIFT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DEPENDENTS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    DEPENDENT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BIRTH_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NATIONAL_ID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IS_ELIGIBLE_FOR_TICKET = table.Column<byte>(type: "tinyint", nullable: false),
                    IS_ELIGIBLE_FOR_INSURANCE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEPENDENTS", x => x.DEPENDENT_ID);
                    table.ForeignKey(
                        name: "FK_DEPENDENTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMERGENCY_CONTACTS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    CONTACT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    CONTACT_NAME_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PHONE_PRIMARY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PHONE_SECONDARY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IS_PRIMARY = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMERGENCY_CONTACTS", x => x.CONTACT_ID);
                    table.ForeignKey(
                        name: "FK_EMERGENCY_CONTACTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_ADDRESSES",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    ADDRESS_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    ADDRESS_TYPE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CITY_ID = table.Column<int>(type: "int", nullable: true),
                    DISTRICT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    STREET = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BUILDING_NO = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    POSTAL_CODE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_ADDRESSES", x => x.ADDRESS_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_ADDRESSES_CITIES_CITY_ID",
                        column: x => x.CITY_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "CITIES",
                        principalColumn: "CITY_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_ADDRESSES_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_APPRAISALS",
                schema: "HR_PERFORMANCE",
                columns: table => new
                {
                    APPRAISAL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    CYCLE_ID = table.Column<int>(type: "int", nullable: false),
                    EVALUATOR_ID = table.Column<int>(type: "int", nullable: false),
                    APPRAISAL_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FINAL_SCORE = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    GRADE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EMPLOYEE_COMMENT = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_APPRAISALS", x => x.APPRAISAL_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_APPRAISALS_APPRAISAL_CYCLES_CYCLE_ID",
                        column: x => x.CYCLE_ID,
                        principalSchema: "HR_PERFORMANCE",
                        principalTable: "APPRAISAL_CYCLES",
                        principalColumn: "CYCLE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_APPRAISALS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_APPRAISALS_EMPLOYEES_EVALUATOR_ID",
                        column: x => x.EVALUATOR_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_BANK_ACCOUNTS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    BANK_ID = table.Column<int>(type: "int", nullable: false),
                    ACCOUNT_NUMBER = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IS_PRIMARY = table.Column<byte>(type: "tinyint", nullable: false),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_BANK_ACCOUNTS", x => x.ACCOUNT_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_BANK_ACCOUNTS_BANKS_BANK_ID",
                        column: x => x.BANK_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "BANKS",
                        principalColumn: "BANK_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_BANK_ACCOUNTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_CERTIFICATIONS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    CERT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    CERT_NAME_AR = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ISSUING_AUTHORITY = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ISSUE_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EXPIRY_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CERT_NUMBER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IS_MANDATORY = table.Column<byte>(type: "tinyint", nullable: false),
                    ATTACHMENT_PATH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_CERTIFICATIONS", x => x.CERT_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_CERTIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_DOCUMENTS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    DOC_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    DOC_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    DOC_NUMBER = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ISSUE_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EXPIRY_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ISSUE_PLACE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ATTACHMENT_PATH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IS_ACTIVE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_DOCUMENTS", x => x.DOC_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_DOCUMENTS_DOCUMENT_TYPES_DOC_TYPE_ID",
                        column: x => x.DOC_TYPE_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "DOC_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_DOCUMENTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_EXPERIENCES",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    EXPERIENCE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    COMPANY_NAME_AR = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JOB_TITLE_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_CURRENT = table.Column<byte>(type: "tinyint", nullable: false),
                    RESPONSIBILITIES = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    REASON_FOR_LEAVING = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_EXPERIENCES", x => x.EXPERIENCE_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_EXPERIENCES_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_LEAVE_BALANCES",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    BALANCE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    LEAVE_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    CURRENT_BALANCE = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    YEAR = table.Column<short>(type: "smallint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_LEAVE_BALANCES", x => x.BALANCE_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_LEAVE_BALANCES_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_LEAVE_BALANCES_LEAVE_TYPES_LEAVE_TYPE_ID",
                        column: x => x.LEAVE_TYPE_ID,
                        principalSchema: "HR_LEAVES",
                        principalTable: "LEAVE_TYPES",
                        principalColumn: "LEAVE_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_QUALIFICATIONS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    QUALIFICATION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    DEGREE_TYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MAJOR_AR = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UNIVERSITY_AR = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    COUNTRY_ID = table.Column<int>(type: "int", nullable: true),
                    GRADUATION_YEAR = table.Column<short>(type: "smallint", nullable: true),
                    GRADE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ATTACHMENT_PATH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_QUALIFICATIONS", x => x.QUALIFICATION_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_QUALIFICATIONS_COUNTRIES_COUNTRY_ID",
                        column: x => x.COUNTRY_ID,
                        principalSchema: "HR_CORE",
                        principalTable: "COUNTRIES",
                        principalColumn: "COUNTRY_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_QUALIFICATIONS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_ROSTERS",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    ROSTER_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    ROSTER_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SHIFT_ID = table.Column<int>(type: "int", nullable: true),
                    IS_OFF_DAY = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_ROSTERS", x => x.ROSTER_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_ROSTERS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_ROSTERS_SHIFT_TYPES_SHIFT_ID",
                        column: x => x.SHIFT_ID,
                        principalSchema: "HR_ATTENDANCE",
                        principalTable: "SHIFT_TYPES",
                        principalColumn: "SHIFT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_SALARY_STRUCTURE",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    StructureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    ELEMENT_ID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_SALARY_STRUCTURE", x => x.StructureId);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_SALARY_STRUCTURE_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_SALARY_STRUCTURE_SALARY_ELEMENTS_ELEMENT_ID",
                        column: x => x.ELEMENT_ID,
                        principalSchema: "HR_PAYROLL",
                        principalTable: "SALARY_ELEMENTS",
                        principalColumn: "ElementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_VIOLATIONS",
                schema: "HR_PERFORMANCE",
                columns: table => new
                {
                    VIOLATION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    VIOLATION_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    VIOLATION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ACTION_ID = table.Column<int>(type: "int", nullable: true),
                    IS_EXECUTED = table.Column<byte>(type: "tinyint", nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    INVESTIGATION_NOTES = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_VIOLATIONS", x => x.VIOLATION_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_VIOLATIONS_DISCIPLINARY_ACTIONS_ACTION_ID",
                        column: x => x.ACTION_ID,
                        principalSchema: "HR_PERFORMANCE",
                        principalTable: "DISCIPLINARY_ACTIONS",
                        principalColumn: "ACTION_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_VIOLATIONS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_VIOLATIONS_VIOLATION_TYPES_VIOLATION_TYPE_ID",
                        column: x => x.VIOLATION_TYPE_ID,
                        principalSchema: "HR_PERFORMANCE",
                        principalTable: "VIOLATION_TYPES",
                        principalColumn: "VIOLATION_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "END_OF_SERVICE_CALC",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    EosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceYears = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    LastBasicSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CalculationNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPaid = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_END_OF_SERVICE_CALC", x => x.EosId);
                    table.ForeignKey(
                        name: "FK_END_OF_SERVICE_CALC_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEAVE_ENCASHMENT",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    ENCASH_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    LEAVE_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    DAYS_ENCASHED = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    AMOUNT = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ENCASH_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PAYROLL_RUN_ID = table.Column<int>(type: "int", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVE_ENCASHMENT", x => x.ENCASH_ID);
                    table.ForeignKey(
                        name: "FK_LEAVE_ENCASHMENT_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LEAVE_ENCASHMENT_LEAVE_TYPES_LEAVE_TYPE_ID",
                        column: x => x.LEAVE_TYPE_ID,
                        principalSchema: "HR_LEAVES",
                        principalTable: "LEAVE_TYPES",
                        principalColumn: "LEAVE_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEAVE_REQUESTS",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    LEAVE_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DAYS_COUNT = table.Column<int>(type: "int", nullable: false),
                    REASON = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ATTACHMENT_PATH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    REJECTION_REASON = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IS_POSTED_TO_BALANCE = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVE_REQUESTS", x => x.REQUEST_ID);
                    table.ForeignKey(
                        name: "FK_LEAVE_REQUESTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LEAVE_REQUESTS_LEAVE_TYPES_LEAVE_TYPE_ID",
                        column: x => x.LEAVE_TYPE_ID,
                        principalSchema: "HR_LEAVES",
                        principalTable: "LEAVE_TYPES",
                        principalColumn: "LEAVE_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEAVE_TRANSACTIONS",
                schema: "HR_LEAVES",
                columns: table => new
                {
                    TRANSACTION_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    LEAVE_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_TYPE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DAYS = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TRANSACTION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    REFERENCE_ID = table.Column<int>(type: "int", nullable: true),
                    NOTES = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAVE_TRANSACTIONS", x => x.TRANSACTION_ID);
                    table.ForeignKey(
                        name: "FK_LEAVE_TRANSACTIONS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LEAVE_TRANSACTIONS_LEAVE_TYPES_LEAVE_TYPE_ID",
                        column: x => x.LEAVE_TYPE_ID,
                        principalSchema: "HR_LEAVES",
                        principalTable: "LEAVE_TYPES",
                        principalColumn: "LEAVE_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LOANS",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    LoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InstallmentCount = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOANS", x => x.LoanId);
                    table.ForeignKey(
                        name: "FK_LOANS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OVERTIME_REQUESTS",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    OT_REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    REQUEST_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WORK_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HOURS_REQUESTED = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    REASON = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    APPROVED_BY = table.Column<int>(type: "int", nullable: true),
                    APPROVED_HOURS = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OVERTIME_REQUESTS", x => x.OT_REQUEST_ID);
                    table.ForeignKey(
                        name: "FK_OVERTIME_REQUESTS_EMPLOYEES_APPROVED_BY",
                        column: x => x.APPROVED_BY,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OVERTIME_REQUESTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAYROLL_ADJUSTMENTS",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    AdjustmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    PAYROLL_RUN_ID = table.Column<int>(type: "int", nullable: true),
                    AdjustmentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYROLL_ADJUSTMENTS", x => x.AdjustmentId);
                    table.ForeignKey(
                        name: "FK_PAYROLL_ADJUSTMENTS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PAYROLL_ADJUSTMENTS_PAYROLL_RUNS_PAYROLL_RUN_ID",
                        column: x => x.PAYROLL_RUN_ID,
                        principalSchema: "HR_PAYROLL",
                        principalTable: "PAYROLL_RUNS",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAYSLIPS",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    PayslipId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RUN_ID = table.Column<int>(type: "int", nullable: false),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalAllowances = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalDeductions = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    NetSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYSLIPS", x => x.PayslipId);
                    table.ForeignKey(
                        name: "FK_PAYSLIPS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PAYSLIPS_PAYROLL_RUNS_RUN_ID",
                        column: x => x.RUN_ID,
                        principalSchema: "HR_PAYROLL",
                        principalTable: "PAYROLL_RUNS",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RAW_PUNCH_LOGS",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    LOG_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    DEVICE_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PUNCH_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PUNCH_TYPE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IS_PROCESSED = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RAW_PUNCH_LOGS", x => x.LOG_ID);
                    table.ForeignKey(
                        name: "FK_RAW_PUNCH_LOGS_EMPLOYEES_EMPLOYEE_ID",
                        column: x => x.EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SHIFT_SWAP_REQUESTS",
                schema: "HR_ATTENDANCE",
                columns: table => new
                {
                    REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    REQUESTER_ID = table.Column<int>(type: "int", nullable: false),
                    TARGET_EMPLOYEE_ID = table.Column<int>(type: "int", nullable: false),
                    ROSTER_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MANAGER_COMMENT = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHIFT_SWAP_REQUESTS", x => x.REQUEST_ID);
                    table.ForeignKey(
                        name: "FK_SHIFT_SWAP_REQUESTS_EMPLOYEES_REQUESTER_ID",
                        column: x => x.REQUESTER_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SHIFT_SWAP_REQUESTS_EMPLOYEES_TARGET_EMPLOYEE_ID",
                        column: x => x.TARGET_EMPLOYEE_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "APPLICATIONS",
                schema: "HR_RECRUITMENT",
                columns: table => new
                {
                    APP_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VACANCY_ID = table.Column<int>(type: "int", nullable: false),
                    CANDIDATE_ID = table.Column<int>(type: "int", nullable: false),
                    APPLICATION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    REJECTION_REASON = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RX_SOURCE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPLICATIONS", x => x.APP_ID);
                    table.ForeignKey(
                        name: "FK_APPLICATIONS_CANDIDATES_CANDIDATE_ID",
                        column: x => x.CANDIDATE_ID,
                        principalSchema: "HR_RECRUITMENT",
                        principalTable: "CANDIDATES",
                        principalColumn: "CANDIDATE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_APPLICATIONS_JOB_VACANCIES_VACANCY_ID",
                        column: x => x.VACANCY_ID,
                        principalSchema: "HR_RECRUITMENT",
                        principalTable: "JOB_VACANCIES",
                        principalColumn: "VACANCY_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONTRACT_RENEWALS",
                schema: "HR_PERSONNEL",
                columns: table => new
                {
                    RENEWAL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CONTRACT_ID = table.Column<int>(type: "int", nullable: false),
                    OLD_END_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NEW_START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NEW_END_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RENEWAL_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NOTES = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRACT_RENEWALS", x => x.RENEWAL_ID);
                    table.ForeignKey(
                        name: "FK_CONTRACT_RENEWALS_CONTRACTS_CONTRACT_ID",
                        column: x => x.CONTRACT_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "CONTRACTS",
                        principalColumn: "CONTRACT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "APPRAISAL_DETAILS",
                schema: "HR_PERFORMANCE",
                columns: table => new
                {
                    DETAIL_ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    APPRAISAL_ID = table.Column<int>(type: "int", nullable: false),
                    KPI_ID = table.Column<int>(type: "int", nullable: false),
                    TARGET_VALUE = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ACTUAL_VALUE = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SCORE = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    COMMENTS = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPRAISAL_DETAILS", x => x.DETAIL_ID);
                    table.ForeignKey(
                        name: "FK_APPRAISAL_DETAILS_EMPLOYEE_APPRAISALS_APPRAISAL_ID",
                        column: x => x.APPRAISAL_ID,
                        principalSchema: "HR_PERFORMANCE",
                        principalTable: "EMPLOYEE_APPRAISALS",
                        principalColumn: "APPRAISAL_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_APPRAISAL_DETAILS_KPI_LIBRARIES_KPI_ID",
                        column: x => x.KPI_ID,
                        principalSchema: "HR_PERFORMANCE",
                        principalTable: "KPI_LIBRARIES",
                        principalColumn: "KPI_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LOAN_INSTALLMENTS",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    InstallmentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LOAN_ID = table.Column<int>(type: "int", nullable: false),
                    InstallmentNumber = table.Column<short>(type: "smallint", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsPaid = table.Column<byte>(type: "tinyint", nullable: false),
                    PAID_IN_PAYROLL_RUN = table.Column<int>(type: "int", nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAN_INSTALLMENTS", x => x.InstallmentId);
                    table.ForeignKey(
                        name: "FK_LOAN_INSTALLMENTS_LOANS_LOAN_ID",
                        column: x => x.LOAN_ID,
                        principalSchema: "HR_PAYROLL",
                        principalTable: "LOANS",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAYSLIP_DETAILS",
                schema: "HR_PAYROLL",
                columns: table => new
                {
                    DetailId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PAYSLIP_ID = table.Column<long>(type: "bigint", nullable: false),
                    ELEMENT_ID = table.Column<int>(type: "int", nullable: false),
                    ElementNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYSLIP_DETAILS", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_PAYSLIP_DETAILS_PAYSLIPS_PAYSLIP_ID",
                        column: x => x.PAYSLIP_ID,
                        principalSchema: "HR_PAYROLL",
                        principalTable: "PAYSLIPS",
                        principalColumn: "PayslipId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PAYSLIP_DETAILS_SALARY_ELEMENTS_ELEMENT_ID",
                        column: x => x.ELEMENT_ID,
                        principalSchema: "HR_PAYROLL",
                        principalTable: "SALARY_ELEMENTS",
                        principalColumn: "ElementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "INTERVIEWS",
                schema: "HR_RECRUITMENT",
                columns: table => new
                {
                    INTERVIEW_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    APP_ID = table.Column<int>(type: "int", nullable: false),
                    INTERVIEWER_ID = table.Column<int>(type: "int", nullable: true),
                    SCHEDULED_TIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    INTERVIEW_TYPE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RATING = table.Column<byte>(type: "tinyint", nullable: true),
                    FEEDBACK = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RESULT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INTERVIEWS", x => x.INTERVIEW_ID);
                    table.ForeignKey(
                        name: "FK_INTERVIEWS_APPLICATIONS_APP_ID",
                        column: x => x.APP_ID,
                        principalSchema: "HR_RECRUITMENT",
                        principalTable: "APPLICATIONS",
                        principalColumn: "APP_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INTERVIEWS_EMPLOYEES_INTERVIEWER_ID",
                        column: x => x.INTERVIEWER_ID,
                        principalSchema: "HR_PERSONNEL",
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OFFERS",
                schema: "HR_RECRUITMENT",
                columns: table => new
                {
                    OFFER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    APP_ID = table.Column<int>(type: "int", nullable: false),
                    OFFER_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BASIC_SALARY = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    HOUSING_ALLOWANCE = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TRANSPORT_ALLOWANCE = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    JOINING_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OFFERS", x => x.OFFER_ID);
                    table.ForeignKey(
                        name: "FK_OFFERS_APPLICATIONS_APP_ID",
                        column: x => x.APP_ID,
                        principalSchema: "HR_RECRUITMENT",
                        principalTable: "APPLICATIONS",
                        principalColumn: "APP_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPLICATIONS_CANDIDATE_ID",
                schema: "HR_RECRUITMENT",
                table: "APPLICATIONS",
                column: "CANDIDATE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_APPLICATIONS_VACANCY_ID",
                schema: "HR_RECRUITMENT",
                table: "APPLICATIONS",
                column: "VACANCY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_APPRAISAL_DETAILS_APPRAISAL_ID",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS",
                column: "APPRAISAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_APPRAISAL_DETAILS_KPI_ID",
                schema: "HR_PERFORMANCE",
                table: "APPRAISAL_DETAILS",
                column: "KPI_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ATTENDANCE_POLICIES_DEPT_ID",
                schema: "HR_ATTENDANCE",
                table: "ATTENDANCE_POLICIES",
                column: "DEPT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ATTENDANCE_POLICIES_JOB_ID",
                schema: "HR_ATTENDANCE",
                table: "ATTENDANCE_POLICIES",
                column: "JOB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CANDIDATES_NATIONALITY_ID",
                schema: "HR_RECRUITMENT",
                table: "CANDIDATES",
                column: "NATIONALITY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CITIES_COUNTRY_ID",
                schema: "HR_CORE",
                table: "CITIES",
                column: "COUNTRY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRACT_RENEWALS_CONTRACT_ID",
                schema: "HR_PERSONNEL",
                table: "CONTRACT_RENEWALS",
                column: "CONTRACT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRACTS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "CONTRACTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DAILY_ATTENDANCE_EMPLOYEE_ID",
                schema: "HR_ATTENDANCE",
                table: "DAILY_ATTENDANCE",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DAILY_ATTENDANCE_PLANNED_SHIFT_ID",
                schema: "HR_ATTENDANCE",
                table: "DAILY_ATTENDANCE",
                column: "PLANNED_SHIFT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEPARTMENTS_PARENT_DEPT_ID",
                schema: "HR_CORE",
                table: "DEPARTMENTS",
                column: "PARENT_DEPT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEPENDENTS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "DEPENDENTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMERGENCY_CONTACTS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMERGENCY_CONTACTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_ADDRESSES_CITY_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_ADDRESSES",
                column: "CITY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_ADDRESSES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_ADDRESSES",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_APPRAISALS_CYCLE_ID",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_APPRAISALS",
                column: "CYCLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_APPRAISALS_EMPLOYEE_ID",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_APPRAISALS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_APPRAISALS_EVALUATOR_ID",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_APPRAISALS",
                column: "EVALUATOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_BANK_ACCOUNTS_BANK_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_BANK_ACCOUNTS",
                column: "BANK_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_BANK_ACCOUNTS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_BANK_ACCOUNTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_CERTIFICATIONS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_CERTIFICATIONS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_DOCUMENTS_DOC_TYPE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                column: "DOC_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_DOCUMENTS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_DOCUMENTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_EXPERIENCES_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_EXPERIENCES",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_LEAVE_BALANCES_EMPLOYEE_ID",
                schema: "HR_LEAVES",
                table: "EMPLOYEE_LEAVE_BALANCES",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_LEAVE_BALANCES_LEAVE_TYPE_ID",
                schema: "HR_LEAVES",
                table: "EMPLOYEE_LEAVE_BALANCES",
                column: "LEAVE_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_QUALIFICATIONS_COUNTRY_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_QUALIFICATIONS",
                column: "COUNTRY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_QUALIFICATIONS_EMPLOYEE_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEE_QUALIFICATIONS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_ROSTERS_EMPLOYEE_ID",
                schema: "HR_ATTENDANCE",
                table: "EMPLOYEE_ROSTERS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_ROSTERS_SHIFT_ID",
                schema: "HR_ATTENDANCE",
                table: "EMPLOYEE_ROSTERS",
                column: "SHIFT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_SALARY_STRUCTURE_ELEMENT_ID",
                schema: "HR_PAYROLL",
                table: "EMPLOYEE_SALARY_STRUCTURE",
                column: "ELEMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_SALARY_STRUCTURE_EMPLOYEE_ID",
                schema: "HR_PAYROLL",
                table: "EMPLOYEE_SALARY_STRUCTURE",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_VIOLATIONS_ACTION_ID",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_VIOLATIONS",
                column: "ACTION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_VIOLATIONS_EMPLOYEE_ID",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_VIOLATIONS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_VIOLATIONS_VIOLATION_TYPE_ID",
                schema: "HR_PERFORMANCE",
                table: "EMPLOYEE_VIOLATIONS",
                column: "VIOLATION_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEES_DEPT_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                column: "DEPT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEES_JOB_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                column: "JOB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEES_MANAGER_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                column: "MANAGER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEES_NATIONALITY_ID",
                schema: "HR_PERSONNEL",
                table: "EMPLOYEES",
                column: "NATIONALITY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_END_OF_SERVICE_CALC_EMPLOYEE_ID",
                schema: "HR_PAYROLL",
                table: "END_OF_SERVICE_CALC",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_INTERVIEWS_APP_ID",
                schema: "HR_RECRUITMENT",
                table: "INTERVIEWS",
                column: "APP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_INTERVIEWS_INTERVIEWER_ID",
                schema: "HR_RECRUITMENT",
                table: "INTERVIEWS",
                column: "INTERVIEWER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_VACANCIES_DEPT_ID",
                schema: "HR_RECRUITMENT",
                table: "JOB_VACANCIES",
                column: "DEPT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_VACANCIES_JOB_ID",
                schema: "HR_RECRUITMENT",
                table: "JOB_VACANCIES",
                column: "JOB_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOBS_DEFAULT_GRADE_ID",
                schema: "HR_CORE",
                table: "JOBS",
                column: "DEFAULT_GRADE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOBS_DepartmentDeptId",
                schema: "HR_CORE",
                table: "JOBS",
                column: "DepartmentDeptId");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_ACCRUAL_RULES_LEAVE_TYPE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_ACCRUAL_RULES",
                column: "LEAVE_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_ENCASHMENT_EMPLOYEE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_ENCASHMENT",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_ENCASHMENT_LEAVE_TYPE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_ENCASHMENT",
                column: "LEAVE_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_REQUESTS_EMPLOYEE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_REQUESTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_REQUESTS_LEAVE_TYPE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_REQUESTS",
                column: "LEAVE_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_TRANSACTIONS_EMPLOYEE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_TRANSACTIONS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEAVE_TRANSACTIONS_LEAVE_TYPE_ID",
                schema: "HR_LEAVES",
                table: "LEAVE_TRANSACTIONS",
                column: "LEAVE_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LOAN_INSTALLMENTS_LOAN_ID",
                schema: "HR_PAYROLL",
                table: "LOAN_INSTALLMENTS",
                column: "LOAN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LOANS_EMPLOYEE_ID",
                schema: "HR_PAYROLL",
                table: "LOANS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OFFERS_APP_ID",
                schema: "HR_RECRUITMENT",
                table: "OFFERS",
                column: "APP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OVERTIME_REQUESTS_APPROVED_BY",
                schema: "HR_ATTENDANCE",
                table: "OVERTIME_REQUESTS",
                column: "APPROVED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_OVERTIME_REQUESTS_EMPLOYEE_ID",
                schema: "HR_ATTENDANCE",
                table: "OVERTIME_REQUESTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYROLL_ADJUSTMENTS_EMPLOYEE_ID",
                schema: "HR_PAYROLL",
                table: "PAYROLL_ADJUSTMENTS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYROLL_ADJUSTMENTS_PAYROLL_RUN_ID",
                schema: "HR_PAYROLL",
                table: "PAYROLL_ADJUSTMENTS",
                column: "PAYROLL_RUN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYSLIP_DETAILS_ELEMENT_ID",
                schema: "HR_PAYROLL",
                table: "PAYSLIP_DETAILS",
                column: "ELEMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYSLIP_DETAILS_PAYSLIP_ID",
                schema: "HR_PAYROLL",
                table: "PAYSLIP_DETAILS",
                column: "PAYSLIP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYSLIPS_EMPLOYEE_ID",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYSLIPS_RUN_ID",
                schema: "HR_PAYROLL",
                table: "PAYSLIPS",
                column: "RUN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RAW_PUNCH_LOGS_EMPLOYEE_ID",
                schema: "HR_ATTENDANCE",
                table: "RAW_PUNCH_LOGS",
                column: "EMPLOYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SHIFT_SWAP_REQUESTS_REQUESTER_ID",
                schema: "HR_ATTENDANCE",
                table: "SHIFT_SWAP_REQUESTS",
                column: "REQUESTER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SHIFT_SWAP_REQUESTS_TARGET_EMPLOYEE_ID",
                schema: "HR_ATTENDANCE",
                table: "SHIFT_SWAP_REQUESTS",
                column: "TARGET_EMPLOYEE_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APPRAISAL_DETAILS",
                schema: "HR_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "ATTENDANCE_POLICIES",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "AUDIT_LOGS",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "CONTRACT_RENEWALS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "DAILY_ATTENDANCE",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "DEPENDENTS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMERGENCY_CONTACTS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_ADDRESSES",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_BANK_ACCOUNTS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_CERTIFICATIONS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_DOCUMENTS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_EXPERIENCES",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_LEAVE_BALANCES",
                schema: "HR_LEAVES");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_QUALIFICATIONS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_ROSTERS",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_SALARY_STRUCTURE",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_VIOLATIONS",
                schema: "HR_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "END_OF_SERVICE_CALC",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "INTERVIEWS",
                schema: "HR_RECRUITMENT");

            migrationBuilder.DropTable(
                name: "LEAVE_ACCRUAL_RULES",
                schema: "HR_LEAVES");

            migrationBuilder.DropTable(
                name: "LEAVE_ENCASHMENT",
                schema: "HR_LEAVES");

            migrationBuilder.DropTable(
                name: "LEAVE_REQUESTS",
                schema: "HR_LEAVES");

            migrationBuilder.DropTable(
                name: "LEAVE_TRANSACTIONS",
                schema: "HR_LEAVES");

            migrationBuilder.DropTable(
                name: "LOAN_INSTALLMENTS",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "NOTIFICATIONS",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "OFFERS",
                schema: "HR_RECRUITMENT");

            migrationBuilder.DropTable(
                name: "OVERTIME_REQUESTS",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "PAYROLL_ADJUSTMENTS",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "PAYSLIP_DETAILS",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "PUBLIC_HOLIDAYS",
                schema: "HR_LEAVES");

            migrationBuilder.DropTable(
                name: "RAW_PUNCH_LOGS",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "REPORT_TEMPLATES",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "ROSTER_PERIODS",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "SHIFT_SWAP_REQUESTS",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "SYSTEM_SETTINGS",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "WORKFLOW_APPROVALS",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_APPRAISALS",
                schema: "HR_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "KPI_LIBRARIES",
                schema: "HR_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "CONTRACTS",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "CITIES",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "BANKS",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "DOCUMENT_TYPES",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "SHIFT_TYPES",
                schema: "HR_ATTENDANCE");

            migrationBuilder.DropTable(
                name: "DISCIPLINARY_ACTIONS",
                schema: "HR_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "VIOLATION_TYPES",
                schema: "HR_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "LEAVE_TYPES",
                schema: "HR_LEAVES");

            migrationBuilder.DropTable(
                name: "LOANS",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "APPLICATIONS",
                schema: "HR_RECRUITMENT");

            migrationBuilder.DropTable(
                name: "PAYSLIPS",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "SALARY_ELEMENTS",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "APPRAISAL_CYCLES",
                schema: "HR_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "CANDIDATES",
                schema: "HR_RECRUITMENT");

            migrationBuilder.DropTable(
                name: "JOB_VACANCIES",
                schema: "HR_RECRUITMENT");

            migrationBuilder.DropTable(
                name: "EMPLOYEES",
                schema: "HR_PERSONNEL");

            migrationBuilder.DropTable(
                name: "PAYROLL_RUNS",
                schema: "HR_PAYROLL");

            migrationBuilder.DropTable(
                name: "COUNTRIES",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "JOBS",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "DEPARTMENTS",
                schema: "HR_CORE");

            migrationBuilder.DropTable(
                name: "JOB_GRADES",
                schema: "HR_CORE");
        }
    }
}
