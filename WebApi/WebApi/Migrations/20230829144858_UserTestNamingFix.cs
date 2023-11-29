using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UserTestNamingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_tests_tests_TestId_TestLanguage",
                table: "user_tests");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tests_users_UserId",
                table: "user_tests");

            migrationBuilder.DropIndex(
                name: "IX_user_tests_UserId",
                table: "user_tests");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "user_tests");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "user_tests");

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "user_tests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_tests_tests_TestId_TestLanguage",
                table: "user_tests",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_tests_tests_TestId_TestLanguage",
                table: "user_tests");

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "user_tests",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "user_tests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<List<string>>(
                name: "Tags",
                table: "user_tests",
                type: "text[]",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_tests_UserId",
                table: "user_tests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_tests_tests_TestId_TestLanguage",
                table: "user_tests",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_tests_users_UserId",
                table: "user_tests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
