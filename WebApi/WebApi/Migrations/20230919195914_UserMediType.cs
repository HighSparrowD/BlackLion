using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UserMediType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMediaPhoto",
                table: "user_data");

            migrationBuilder.AddColumn<short>(
                name: "MediaType",
                table: "user_data",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "user_data");

            migrationBuilder.AddColumn<bool>(
                name: "IsMediaPhoto",
                table: "user_data",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
