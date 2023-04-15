using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddTimestampsToSyncResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClockAdjustedPicoTimestamp",
                table: "SyncBoardResults",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncFinishedTimestamp",
                table: "SyncBoardResults",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClockAdjustedPicoTimestamp",
                table: "SyncBoardResults");

            migrationBuilder.DropColumn(
                name: "SyncFinishedTimestamp",
                table: "SyncBoardResults");
        }
    }
}
