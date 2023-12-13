using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class activeeffects : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresIn",
                table: "active_effects");

            migrationBuilder.RenameColumn(
                name: "EffectId",
                table: "active_effects",
                newName: "Effect");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Effect",
                table: "active_effects",
                newName: "EffectId");

            migrationBuilder.AddColumn<short>(
                name: "ExpiresIn",
                table: "active_effects",
                type: "smallint",
                nullable: true);
        }
    }
}
