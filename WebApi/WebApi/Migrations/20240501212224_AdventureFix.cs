using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdventureFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "Application",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "AttendeesDescription",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "Gratitude",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "IsAutoReplyText",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "UnwantedAttendeesDescription",
                table: "adventures");

            migrationBuilder.AddColumn<byte>(
                name: "AutoReplyType",
                table: "adventures",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoReplyType",
                table: "adventures");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Application",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttendeesDescription",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gratitude",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoReplyText",
                table: "adventures",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "adventures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnwantedAttendeesDescription",
                table: "adventures",
                type: "text",
                nullable: true);
        }
    }
}
