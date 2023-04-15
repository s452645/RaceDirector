using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddHeatToHeatResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeats_SeasonEventRoundRaceHeatId",
                table: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeatId",
                table: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.DropColumn(
                name: "SeasonEventRoundRaceHeatId",
                table: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.AddColumn<Guid>(
                name: "HeatId",
                table: "SeasonEventRoundRaceHeatResults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_HeatId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "HeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeats_HeatId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "HeatId",
                principalTable: "SeasonEventRoundRaceHeats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeats_HeatId",
                table: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_HeatId",
                table: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.DropColumn(
                name: "HeatId",
                table: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.AddColumn<Guid>(
                name: "SeasonEventRoundRaceHeatId",
                table: "SeasonEventRoundRaceHeatResults",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeatId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "SeasonEventRoundRaceHeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeats_SeasonEventRoundRaceHeatId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "SeasonEventRoundRaceHeatId",
                principalTable: "SeasonEventRoundRaceHeats",
                principalColumn: "Id");
        }
    }
}
