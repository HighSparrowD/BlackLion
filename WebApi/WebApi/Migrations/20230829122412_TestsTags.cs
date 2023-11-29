using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TestsTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "tags_hilo");

            migrationBuilder.AddColumn<List<long>>(
                name: "ReplaceableTags",
                table: "tests_results",
                type: "bigint[]",
                nullable: true);

            migrationBuilder.AddColumn<List<long>>(
                name: "ReplaceableTags",
                table: "tests_answers",
                type: "bigint[]",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => new { x.Id, x.Type });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropColumn(
                name: "ReplaceableTags",
                table: "tests_results");

            migrationBuilder.DropColumn(
                name: "ReplaceableTags",
                table: "tests_answers");

            migrationBuilder.DropSequence(
                name: "tags_hilo");
        }
    }
}
