using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class FixScoreRulesTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeMultipiler",
                table: "SeasonEventScoreRules",
                newName: "TimeMultiplier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeMultiplier",
                table: "SeasonEventScoreRules",
                newName: "TimeMultipiler");
        }
    }
}
