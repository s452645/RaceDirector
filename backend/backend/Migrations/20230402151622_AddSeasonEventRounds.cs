using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddSeasonEventRounds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecondChanceRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvancesCount = table.Column<int>(type: "int", nullable: false),
                    PointsStrategy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondChanceRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SecondChanceRulesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PointsStrategy = table.Column<int>(type: "int", nullable: false),
                    DroppedCarsPositionDefinementStrategy = table.Column<int>(type: "int", nullable: false),
                    DroppedCarsPointsStrategy = table.Column<int>(type: "int", nullable: false),
                    SeasonEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonEventRounds_SeasonEvents_SeasonEventId",
                        column: x => x.SeasonEventId,
                        principalTable: "SeasonEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonEventRounds_SecondChanceRules_SecondChanceRulesId",
                        column: x => x.SecondChanceRulesId,
                        principalTable: "SecondChanceRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarSeasonEventRound",
                columns: table => new
                {
                    ParticipantsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonEventRoundsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSeasonEventRound", x => new { x.ParticipantsId, x.SeasonEventRoundsId });
                    table.ForeignKey(
                        name: "FK_CarSeasonEventRound_Cars_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarSeasonEventRound_SeasonEventRounds_SeasonEventRoundsId",
                        column: x => x.SeasonEventRoundsId,
                        principalTable: "SeasonEventRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRoundRaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    InstantAdvancements = table.Column<int>(type: "int", nullable: false),
                    SecondChances = table.Column<int>(type: "int", nullable: false),
                    SeasonEventRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventRoundRaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonEventRoundRaces_SeasonEventRounds_SeasonEventRoundId",
                        column: x => x.SeasonEventRoundId,
                        principalTable: "SeasonEventRounds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRoundRaceHeats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "SeasonEventRoundRaceResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonEventRoundRaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: true),
                    Points = table.Column<float>(type: "real", nullable: false),
                    RaceOutcome = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventRoundRaceResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonEventRoundRaceResults_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonEventRoundRaceResults_SeasonEventRoundRaces_SeasonEventRoundRaceId",
                        column: x => x.SeasonEventRoundRaceId,
                        principalTable: "SeasonEventRoundRaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRoundRaceHeatResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectorTimes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullTime = table.Column<float>(type: "real", nullable: false),
                    TimePoints = table.Column<float>(type: "real", nullable: false),
                    AdvantagePoints = table.Column<float>(type: "real", nullable: false),
                    DistancePoints = table.Column<float>(type: "real", nullable: false),
                    Bonuses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PointsSummed = table.Column<float>(type: "real", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    SeasonEventRoundRaceHeatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                        name: "FK_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeats_SeasonEventRoundRaceHeatId",
                        column: x => x.SeasonEventRoundRaceHeatId,
                        principalTable: "SeasonEventRoundRaceHeats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarSeasonEventRound_SeasonEventRoundsId",
                table: "CarSeasonEventRound",
                column: "SeasonEventRoundsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_CarId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeatResults_SeasonEventRoundRaceHeatId",
                table: "SeasonEventRoundRaceHeatResults",
                column: "SeasonEventRoundRaceHeatId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceHeats_RaceId",
                table: "SeasonEventRoundRaceHeats",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceResults_CarId",
                table: "SeasonEventRoundRaceResults",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceResults_SeasonEventRoundRaceId",
                table: "SeasonEventRoundRaceResults",
                column: "SeasonEventRoundRaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaces_SeasonEventRoundId",
                table: "SeasonEventRoundRaces",
                column: "SeasonEventRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRounds_SeasonEventId",
                table: "SeasonEventRounds",
                column: "SeasonEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRounds_SecondChanceRulesId",
                table: "SeasonEventRounds",
                column: "SecondChanceRulesId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarSeasonEventRound");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceResults");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceHeats");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaces");

            migrationBuilder.DropTable(
                name: "SeasonEventRounds");

            migrationBuilder.DropTable(
                name: "SecondChanceRules");
        }
    }
}
