using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddTrackEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrackNumber",
                table: "SeasonEventRoundRaceHeatResults",
                newName: "Track");

            migrationBuilder.RenameColumn(
                name: "TrackNumber",
                table: "Checkpoints",
                newName: "Track");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Track",
                table: "SeasonEventRoundRaceHeatResults",
                newName: "TrackNumber");

            migrationBuilder.RenameColumn(
                name: "Track",
                table: "Checkpoints",
                newName: "TrackNumber");
        }
    }
}
