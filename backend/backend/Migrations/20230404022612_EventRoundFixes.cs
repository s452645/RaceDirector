using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class EventRoundFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEventRoundRaces_SeasonEventRounds_SeasonEventRoundId",
                table: "SeasonEventRoundRaces");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEventRoundRaces_SeasonEventRoundId",
                table: "SeasonEventRoundRaces");

            migrationBuilder.DropColumn(
                name: "SeasonEventRoundId",
                table: "SeasonEventRoundRaces");

            migrationBuilder.AddColumn<Guid>(
                name: "RoundId",
                table: "SeasonEventRoundRaces",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CarSeasonEvent",
                columns: table => new
                {
                    ParticipantsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonEventsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSeasonEvent", x => new { x.ParticipantsId, x.SeasonEventsId });
                    table.ForeignKey(
                        name: "FK_CarSeasonEvent_Cars_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarSeasonEvent_SeasonEvents_SeasonEventsId",
                        column: x => x.SeasonEventsId,
                        principalTable: "SeasonEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaces_RoundId",
                table: "SeasonEventRoundRaces",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSeasonEvent_SeasonEventsId",
                table: "CarSeasonEvent",
                column: "SeasonEventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEventRoundRaces_SeasonEventRounds_RoundId",
                table: "SeasonEventRoundRaces",
                column: "RoundId",
                principalTable: "SeasonEventRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEventRoundRaces_SeasonEventRounds_RoundId",
                table: "SeasonEventRoundRaces");

            migrationBuilder.DropTable(
                name: "CarSeasonEvent");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEventRoundRaces_RoundId",
                table: "SeasonEventRoundRaces");

            migrationBuilder.DropColumn(
                name: "RoundId",
                table: "SeasonEventRoundRaces");

            migrationBuilder.AddColumn<Guid>(
                name: "SeasonEventRoundId",
                table: "SeasonEventRoundRaces",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaces_SeasonEventRoundId",
                table: "SeasonEventRoundRaces",
                column: "SeasonEventRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEventRoundRaces_SeasonEventRounds_SeasonEventRoundId",
                table: "SeasonEventRoundRaces",
                column: "SeasonEventRoundId",
                principalTable: "SeasonEventRounds",
                principalColumn: "Id");
        }
    }
}
