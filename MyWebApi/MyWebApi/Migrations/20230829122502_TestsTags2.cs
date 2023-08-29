using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TestsTags2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplaceableTags",
                table: "tests_results",
                newName: "Tags");

            migrationBuilder.RenameColumn(
                name: "ReplaceableTags",
                table: "tests_answers",
                newName: "Tags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "tests_results",
                newName: "ReplaceableTags");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "tests_answers",
                newName: "ReplaceableTags");
        }
    }
}
