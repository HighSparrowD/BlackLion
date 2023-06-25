using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class FeedbacksFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonClassLocalisationId",
                table: "feedbacks");

            migrationBuilder.DropColumn(
                name: "ReasonId",
                table: "feedbacks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReasonClassLocalisationId",
                table: "feedbacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "ReasonId",
                table: "feedbacks",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
