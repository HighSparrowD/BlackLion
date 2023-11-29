using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TemplateMediaType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMediaPhoto",
                table: "adventure_templates");

            migrationBuilder.AddColumn<short>(
                name: "MediaType",
                table: "adventure_templates",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "adventure_templates");

            migrationBuilder.AddColumn<bool>(
                name: "IsMediaPhoto",
                table: "adventure_templates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
