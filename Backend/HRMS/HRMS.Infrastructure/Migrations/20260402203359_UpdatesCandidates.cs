using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesCandidates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "APPLICATION_SOURCE",
                schema: "HR_RECRUITMENT",
                table: "CANDIDATES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STATUS",
                schema: "HR_RECRUITMENT",
                table: "CANDIDATES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APPLICATION_SOURCE",
                schema: "HR_RECRUITMENT",
                table: "CANDIDATES");

            migrationBuilder.DropColumn(
                name: "STATUS",
                schema: "HR_RECRUITMENT",
                table: "CANDIDATES");
        }
    }
}
