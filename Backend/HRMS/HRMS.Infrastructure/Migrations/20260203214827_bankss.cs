using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class bankss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ACCOUNTING");

            migrationBuilder.CreateTable(
                name: "ACCOUNTS",
                schema: "ACCOUNTING",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccountNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PARENT_ACCOUNT_ID = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACCOUNTS", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_ACCOUNTS_ACCOUNTS_PARENT_ACCOUNT_ID",
                        column: x => x.PARENT_ACCOUNT_ID,
                        principalSchema: "ACCOUNTING",
                        principalTable: "ACCOUNTS",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JOURNAL_ENTRIES",
                schema: "ACCOUNTING",
                columns: table => new
                {
                    JournalEntryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SourceModule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourceReferenceId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOURNAL_ENTRIES", x => x.JournalEntryId);
                });

            migrationBuilder.CreateTable(
                name: "JOURNAL_ENTRY_LINES",
                schema: "ACCOUNTING",
                columns: table => new
                {
                    LineId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JOURNAL_ENTRY_ID = table.Column<long>(type: "bigint", nullable: false),
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: false),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LineNumber = table.Column<short>(type: "smallint", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_DELETED = table.Column<byte>(type: "tinyint", nullable: false),
                    VERSION_NO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOURNAL_ENTRY_LINES", x => x.LineId);
                    table.ForeignKey(
                        name: "FK_JOURNAL_ENTRY_LINES_ACCOUNTS_ACCOUNT_ID",
                        column: x => x.ACCOUNT_ID,
                        principalSchema: "ACCOUNTING",
                        principalTable: "ACCOUNTS",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JOURNAL_ENTRY_LINES_JOURNAL_ENTRIES_JOURNAL_ENTRY_ID",
                        column: x => x.JOURNAL_ENTRY_ID,
                        principalSchema: "ACCOUNTING",
                        principalTable: "JOURNAL_ENTRIES",
                        principalColumn: "JournalEntryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ACCOUNTS_PARENT_ACCOUNT_ID",
                schema: "ACCOUNTING",
                table: "ACCOUNTS",
                column: "PARENT_ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOURNAL_ENTRY_LINES_ACCOUNT_ID",
                schema: "ACCOUNTING",
                table: "JOURNAL_ENTRY_LINES",
                column: "ACCOUNT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_JOURNAL_ENTRY_LINES_JOURNAL_ENTRY_ID",
                schema: "ACCOUNTING",
                table: "JOURNAL_ENTRY_LINES",
                column: "JOURNAL_ENTRY_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOURNAL_ENTRY_LINES",
                schema: "ACCOUNTING");

            migrationBuilder.DropTable(
                name: "ACCOUNTS",
                schema: "ACCOUNTING");

            migrationBuilder.DropTable(
                name: "JOURNAL_ENTRIES",
                schema: "ACCOUNTING");
        }
    }
}
