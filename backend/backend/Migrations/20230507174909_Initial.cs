using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Circuits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circuits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PicoBoards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IPAddress = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Connected = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PicoBoards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventScoreRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeMultiplier = table.Column<float>(type: "real", nullable: false),
                    DistanceMultiplier = table.Column<float>(type: "real", nullable: false),
                    AvailableBonuses = table.Column<string>(type: "text", nullable: false),
                    UnfinishedSectorPenaltyPoints = table.Column<float>(type: "real", nullable: false),
                    TheMoreTheBetter = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventScoreRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecondChanceRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvancesCount = table.Column<int>(type: "integer", nullable: false),
                    PointsStrategy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondChanceRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstLevelName = table.Column<string>(type: "text", nullable: false),
                    SecondLevelName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BreakBeamSensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Pin = table.Column<int>(type: "integer", nullable: false),
                    BoardId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreakBeamSensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreakBeamSensors_PicoBoards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "PicoBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SyncBoardResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PicoBoardId = table.Column<Guid>(type: "uuid", nullable: false),
                    SyncResult = table.Column<int>(type: "integer", nullable: false),
                    CurrentSyncOffset = table.Column<long>(type: "bigint", nullable: true),
                    LastTenOffsetsAvg = table.Column<float>(type: "real", nullable: true),
                    NewClockOffset = table.Column<long>(type: "bigint", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    ClockAdjustedPicoTimestamp = table.Column<long>(type: "bigint", nullable: true),
                    SyncFinishedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncBoardResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncBoardResults_PicoBoards_PicoBoardId",
                        column: x => x.PicoBoardId,
                        principalTable: "PicoBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ScoreRulesId = table.Column<Guid>(type: "uuid", nullable: true),
                    CircuitId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonEvents_Circuits_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "Circuits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SeasonEvents_SeasonEventScoreRules_ScoreRulesId",
                        column: x => x.ScoreRulesId,
                        principalTable: "SeasonEventScoreRules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SeasonEvents_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Prefix = table.Column<string>(type: "text", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SensorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Broken = table.Column<bool>(type: "boolean", nullable: false),
                    PicoLocalTimestamp = table.Column<long>(type: "bigint", nullable: false),
                    ReceivedTimestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardEvents_BreakBeamSensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "BreakBeamSensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checkpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    BreakBeamSensorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CircuitId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkpoints_BreakBeamSensors_BreakBeamSensorId",
                        column: x => x.BreakBeamSensorId,
                        principalTable: "BreakBeamSensors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Checkpoints_Circuits_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "Circuits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ParticipantsCount = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SecondChanceRulesId = table.Column<Guid>(type: "uuid", nullable: true),
                    PointsStrategy = table.Column<int>(type: "integer", nullable: false),
                    DroppedCarsPositionDefinementStrategy = table.Column<int>(type: "integer", nullable: false),
                    DroppedCarsPointsStrategy = table.Column<int>(type: "integer", nullable: false),
                    SeasonEventId = table.Column<Guid>(type: "uuid", nullable: false)
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Hierarchy = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pots_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonTeamStandings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<double>(type: "double precision", nullable: false),
                    Place = table.Column<int>(type: "integer", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonTeamStandings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonTeamStandings_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SeasonTeamStandings_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRoundRaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ParticipantsCount = table.Column<int>(type: "integer", nullable: false),
                    InstantAdvancements = table.Column<int>(type: "integer", nullable: false),
                    SecondChances = table.Column<int>(type: "integer", nullable: false),
                    RoundId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEventRoundRaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonEventRoundRaces_SeasonEventRounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "SeasonEventRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEventRoundRaceHeats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    RaceId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "CarPot",
                columns: table => new
                {
                    CarsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PotsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPot", x => new { x.CarsId, x.PotsId });
                    table.ForeignKey(
                        name: "FK_CarPot_Pots_PotsId",
                        column: x => x.PotsId,
                        principalTable: "Pots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    MainPhotoId = table.Column<Guid>(type: "uuid", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarSeasonEvent",
                columns: table => new
                {
                    ParticipantsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonEventsId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CarSeasonEventRound",
                columns: table => new
                {
                    ParticipantsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonEventRoundsId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "CarSizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CarId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeightId = table.Column<Guid>(type: "uuid", nullable: true),
                    LengthId = table.Column<Guid>(type: "uuid", nullable: true),
                    WidthId = table.Column<Guid>(type: "uuid", nullable: true),
                    HeightId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarSizes_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarSizes_UnitValues_HeightId",
                        column: x => x.HeightId,
                        principalTable: "UnitValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CarSizes_UnitValues_LengthId",
                        column: x => x.LengthId,
                        principalTable: "UnitValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CarSizes_UnitValues_WeightId",
                        column: x => x.WeightId,
                        principalTable: "UnitValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CarSizes_UnitValues_WidthId",
                        column: x => x.WidthId,
                        principalTable: "UnitValues",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OfficialNames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelName = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: true),
                    WikiLink = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CarId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficialNames_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficialNames_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    File = table.Column<byte[]>(type: "bytea", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CarId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SeasonCarStandings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CarId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<double>(type: "double precision", nullable: false),
                    Place = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonCarStandings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonCarStandings_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonCarStandings_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
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
                    DistancePoints = table.Column<float>(type: "real", nullable: false),
                    Bonuses = table.Column<string>(type: "text", nullable: false),
                    PointsSummed = table.Column<float>(type: "real", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    SectorTimes = table.Column<string>(type: "text", nullable: true),
                    FullTime = table.Column<float>(type: "real", nullable: true),
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
                name: "SeasonEventRoundRaceResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CarId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonEventRoundRaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: true),
                    Points = table.Column<float>(type: "real", nullable: false),
                    RaceOutcome = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_SeasonEventRoundRaceResults_SeasonEventRoundRaces_SeasonEve~",
                        column: x => x.SeasonEventRoundRaceId,
                        principalTable: "SeasonEventRoundRaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uuid", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Prefix = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Owners_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RaceHeatSectorResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<float>(type: "real", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    PositionPoints = table.Column<float>(type: "real", nullable: false),
                    AdvantagePoints = table.Column<float>(type: "real", nullable: false),
                    RaceHeatResultId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "IX_BoardEvents_SensorId",
                table: "BoardEvents",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_BreakBeamSensors_BoardId",
                table: "BreakBeamSensors",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_CarPot_PotsId",
                table: "CarPot",
                column: "PotsId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_MainPhotoId",
                table: "Cars",
                column: "MainPhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_OwnerId",
                table: "Cars",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSeasonEvent_SeasonEventsId",
                table: "CarSeasonEvent",
                column: "SeasonEventsId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSeasonEventRound_SeasonEventRoundsId",
                table: "CarSeasonEventRound",
                column: "SeasonEventRoundsId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSizes_CarId",
                table: "CarSizes",
                column: "CarId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarSizes_HeightId",
                table: "CarSizes",
                column: "HeightId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSizes_LengthId",
                table: "CarSizes",
                column: "LengthId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSizes_WeightId",
                table: "CarSizes",
                column: "WeightId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSizes_WidthId",
                table: "CarSizes",
                column: "WidthId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_BreakBeamSensorId",
                table: "Checkpoints",
                column: "BreakBeamSensorId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_CircuitId",
                table: "Checkpoints",
                column: "CircuitId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialNames_CarId",
                table: "OfficialNames",
                column: "CarId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialNames_SeriesId",
                table: "OfficialNames",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_PhotoId",
                table: "Owners",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CarId",
                table: "Photos",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Pots_TeamId",
                table: "Pots",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceHeatSectorResults_RaceHeatResultId",
                table: "RaceHeatSectorResults",
                column: "RaceHeatResultId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonCarStandings_CarId",
                table: "SeasonCarStandings",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonCarStandings_SeasonId",
                table: "SeasonCarStandings",
                column: "SeasonId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceResults_CarId",
                table: "SeasonEventRoundRaceResults",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaceResults_SeasonEventRoundRaceId",
                table: "SeasonEventRoundRaceResults",
                column: "SeasonEventRoundRaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRoundRaces_RoundId",
                table: "SeasonEventRoundRaces",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRounds_SeasonEventId",
                table: "SeasonEventRounds",
                column: "SeasonEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRounds_SecondChanceRulesId",
                table: "SeasonEventRounds",
                column: "SecondChanceRulesId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEvents_CircuitId",
                table: "SeasonEvents",
                column: "CircuitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEvents_ScoreRulesId",
                table: "SeasonEvents",
                column: "ScoreRulesId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEvents_SeasonId",
                table: "SeasonEvents",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeamStandings_SeasonId",
                table: "SeasonTeamStandings",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeamStandings_TeamId",
                table: "SeasonTeamStandings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncBoardResults_PicoBoardId",
                table: "SyncBoardResults",
                column: "PicoBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SeasonId",
                table: "Teams",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarPot_Cars_CarsId",
                table: "CarPot",
                column: "CarsId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Owners_OwnerId",
                table: "Cars",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Photos_MainPhotoId",
                table: "Cars",
                column: "MainPhotoId",
                principalTable: "Photos",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Cars_CarId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "BoardEvents");

            migrationBuilder.DropTable(
                name: "CarPot");

            migrationBuilder.DropTable(
                name: "CarSeasonEvent");

            migrationBuilder.DropTable(
                name: "CarSeasonEventRound");

            migrationBuilder.DropTable(
                name: "CarSizes");

            migrationBuilder.DropTable(
                name: "Checkpoints");

            migrationBuilder.DropTable(
                name: "OfficialNames");

            migrationBuilder.DropTable(
                name: "RaceHeatSectorResults");

            migrationBuilder.DropTable(
                name: "SeasonCarStandings");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceResults");

            migrationBuilder.DropTable(
                name: "SeasonTeamStandings");

            migrationBuilder.DropTable(
                name: "SyncBoardResults");

            migrationBuilder.DropTable(
                name: "Pots");

            migrationBuilder.DropTable(
                name: "UnitValues");

            migrationBuilder.DropTable(
                name: "BreakBeamSensors");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceHeatResults");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "PicoBoards");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaceHeats");

            migrationBuilder.DropTable(
                name: "SeasonEventRoundRaces");

            migrationBuilder.DropTable(
                name: "SeasonEventRounds");

            migrationBuilder.DropTable(
                name: "SeasonEvents");

            migrationBuilder.DropTable(
                name: "SecondChanceRules");

            migrationBuilder.DropTable(
                name: "Circuits");

            migrationBuilder.DropTable(
                name: "SeasonEventScoreRules");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "Photos");
        }
    }
}
