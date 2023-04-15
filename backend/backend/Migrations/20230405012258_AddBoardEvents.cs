using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddBoardEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Broken = table.Column<bool>(type: "bit", nullable: false),
                    PicoLocalTimestamp = table.Column<long>(type: "bigint", nullable: false),
                    ReceivedTimestamp = table.Column<long>(type: "bigint", nullable: false),
                    PicoBoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_BoardEvents_PicoBoards_PicoBoardId",
                        column: x => x.PicoBoardId,
                        principalTable: "PicoBoards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardEvents_PicoBoardId",
                table: "BoardEvents",
                column: "PicoBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardEvents_SensorId",
                table: "BoardEvents",
                column: "SensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardEvents");
        }
    }
}
