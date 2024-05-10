using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Statuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "verification_requests",
                newName: "Status");

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "advertisements",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "advertisements");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "verification_requests",
                newName: "State");
        }
    }
}
