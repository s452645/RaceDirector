using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class FixBreakBeamSensor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checkpoints_BreakBeamSensorId",
                table: "Checkpoints");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_BreakBeamSensorId",
                table: "Checkpoints",
                column: "BreakBeamSensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checkpoints_BreakBeamSensorId",
                table: "Checkpoints");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_BreakBeamSensorId",
                table: "Checkpoints",
                column: "BreakBeamSensorId",
                unique: true,
                filter: "[BreakBeamSensorId] IS NOT NULL");
        }
    }
}
