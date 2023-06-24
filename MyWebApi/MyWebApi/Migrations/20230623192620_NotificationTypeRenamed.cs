using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTypeRenamed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "notifications");

            migrationBuilder.AddColumn<short>(
                name: "Type",
                table: "notifications",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "notifications");

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
