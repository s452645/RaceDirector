using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class RemoveBoardFromEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardEvents_PicoBoards_PicoBoardId",
                table: "BoardEvents");

            migrationBuilder.DropIndex(
                name: "IX_BoardEvents_PicoBoardId",
                table: "BoardEvents");

            migrationBuilder.DropColumn(
                name: "PicoBoardId",
                table: "BoardEvents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PicoBoardId",
                table: "BoardEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardEvents_PicoBoardId",
                table: "BoardEvents",
                column: "PicoBoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardEvents_PicoBoards_PicoBoardId",
                table: "BoardEvents",
                column: "PicoBoardId",
                principalTable: "PicoBoards",
                principalColumn: "Id");
        }
    }
}
