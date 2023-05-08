using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class ReplaceTracksWithTrackNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkpoints_Tracks_TrackId",
                table: "Checkpoints");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Checkpoints_TrackId",
                table: "Checkpoints");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Checkpoints");

            migrationBuilder.AddColumn<int>(
                name: "TrackNumber",
                table: "Checkpoints",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackNumber",
                table: "Checkpoints");

            migrationBuilder.AddColumn<Guid>(
                name: "TrackId",
                table: "Checkpoints",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_TrackId",
                table: "Checkpoints",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkpoints_Tracks_TrackId",
                table: "Checkpoints",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id");
        }
    }
}
