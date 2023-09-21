using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class RemoveHeats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceHeatSectorResults");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceHeats");

            migrationBuilder.AddColumn<float>(
                name: "BonusesPoints",
                table: "SeasonEventRoundRaceResults",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Distance",
                table: "SeasonEventRoundRaceResults",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "SectorTimes",
                table: "SeasonEventRoundRaceResults",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusesPoints",
                table: "SeasonEventRoundRaceResults");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "SeasonEventRoundRaceResults");

            migrationBuilder.DropColumn(
                name: "SectorTimes",
                table: "SeasonEventRoundRaceResults");

            migrationBuilder.CreateTable(
                name: "SeasonEventRoundRaceHeats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventRoundRaceHeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonEventRoundRaceHeats_SeasonEventRoundRaces_RaceId",
                        column: x => x.RaceId,
                        principalTable: "SeasonEventRoundRaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRoundRaceHeatResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CarId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeatId = table.Column<Guid>(type: "uuid", nullable: false),
                    Bonuses = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    DistancePoints = table.Column<float>(type: "real", nullable: false),
                    PointsSummed = table.Column<float>(type: "real", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Track = table.Column<int>(type: "integer", nullable: false),
                    FullTime = table.Column<float>(type: "real", nullable: true),
                    SectorTimes = table.Column<string>(type: "text", nullable: true),
                    TimePoints = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventRoundRaceHeatResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonEventRoundRaceHeatResults_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeats_H~",
                        column: x => x.HeatId,
                        principalTable: "SeasonEventRoundRaceHeats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceHeatSectorResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RaceHeatResultId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvantagePoints = table.Column<float>(type: "real", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    PositionPoints = table.Column<float>(type: "real", nullable: false),
                    Time = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceHeatSectorResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceHeatSectorResults_SeasonEventRoundRaceHeatResults_RaceH~",
                        column: x => x.RaceHeatResultId,
                        principalTable: "SeasonEventRoundRaceHeatResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaceHeatSectorResults_RaceHeatResultId",
                table: "RaceHeatSectorResults",
                column: "RaceHeatResultId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_CarId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_HeatId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "HeatId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeats_RaceId",
                table: "SeasonEventRoundRaceHeats",
                column: "RaceId");
        }
    }
}
