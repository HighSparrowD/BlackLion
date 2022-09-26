using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyWebApi.Migrations
{
    public partial class userDailyReward : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "DailyRewardPoint",
                table: "SYSTEM_USERS",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HadReceivedReward",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SPONSOR_EVENT_TEMPLATES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Languages = table.Column<List<int?>>(type: "integer[]", nullable: true),
                    MinAge = table.Column<short>(type: "smallint", nullable: false),
                    MaxAge = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Bounty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_EVENT_TEMPLATES", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPONSOR_EVENT_TEMPLATES");

            migrationBuilder.DropColumn(
                name: "DailyRewardPoint",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "HadReceivedReward",
                table: "SYSTEM_USERS");
        }
    }
}
