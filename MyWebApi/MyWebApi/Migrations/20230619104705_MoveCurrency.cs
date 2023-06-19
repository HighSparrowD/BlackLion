using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class MoveCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "users");

            migrationBuilder.AddColumn<short>(
                name: "Currency",
                table: "balances",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "balances");

            migrationBuilder.AddColumn<short>(
                name: "Currency",
                table: "users",
                type: "smallint",
                nullable: true);
        }
    }
}
