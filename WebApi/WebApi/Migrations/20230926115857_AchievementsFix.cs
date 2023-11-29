using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AchievementsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcquireMessage",
                table: "user_achievements");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "user_achievements");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "achievements");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "achievements",
                newName: "Reward");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reward",
                table: "achievements",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "AcquireMessage",
                table: "user_achievements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "user_achievements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "achievements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
