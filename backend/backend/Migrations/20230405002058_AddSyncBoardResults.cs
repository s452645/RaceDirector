using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddSyncBoardResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncBoardResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PicoBoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyncResult = table.Column<int>(type: "int", nullable: false),
                    CurrentSyncOffset = table.Column<int>(type: "int", nullable: true),
                    LastTenOffsetsAvg = table.Column<float>(type: "real", nullable: true),
                    NewClockOffset = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_SyncBoardResults_PicoBoardId",
                table: "SyncBoardResults",
                column: "PicoBoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncBoardResults");
        }
    }
}
