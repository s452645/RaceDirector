using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddSeasonEventScoreRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScoreRulesId",
                table: "SeasonEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "SeasonEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SeasonEventScoreRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeMultipiler = table.Column<float>(type: "real", nullable: false),
                    DistanceMultiplier = table.Column<float>(type: "real", nullable: false),
                    AvailableBonuses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnfinishedSectorPenaltyPoints = table.Column<float>(type: "real", nullable: false),
                    TheMoreTheBetter = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventScoreRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEvents_ScoreRulesId",
                table: "SeasonEvents",
                column: "ScoreRulesId",
                unique: true,
                filter: "[ScoreRulesId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEvents_SeasonEventScoreRules_ScoreRulesId",
                table: "SeasonEvents",
                column: "ScoreRulesId",
                principalTable: "SeasonEventScoreRules",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEvents_SeasonEventScoreRules_ScoreRulesId",
                table: "SeasonEvents");

            migrationBuilder.DropTable(
                name: "SeasonEventScoreRules");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEvents_ScoreRulesId",
                table: "SeasonEvents");

            migrationBuilder.DropColumn(
                name: "ScoreRulesId",
                table: "SeasonEvents");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SeasonEvents");
        }
    }
}
