using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class adventure_mediatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMediaPhoto",
                table: "adventures");

            migrationBuilder.AddColumn<short>(
                name: "MediaType",
                table: "adventures",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "adventures");

            migrationBuilder.AddColumn<bool>(
                name: "IsMediaPhoto",
                table: "adventures",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
