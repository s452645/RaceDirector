using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class MakeSecondChanceRulesOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEventRounds_SecondChanceRules_SecondChanceRulesId",
                table: "SeasonEventRounds");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEventRounds_SecondChanceRulesId",
                table: "SeasonEventRounds");

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondChanceRulesId",
                table: "SeasonEventRounds",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRounds_SecondChanceRulesId",
                table: "SeasonEventRounds",
                column: "SecondChanceRulesId",
                unique: true,
                filter: "[SecondChanceRulesId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEventRounds_SecondChanceRules_SecondChanceRulesId",
                table: "SeasonEventRounds",
                column: "SecondChanceRulesId",
                principalTable: "SecondChanceRules",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEventRounds_SecondChanceRules_SecondChanceRulesId",
                table: "SeasonEventRounds");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEventRounds_SecondChanceRulesId",
                table: "SeasonEventRounds");

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondChanceRulesId",
                table: "SeasonEventRounds",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEventRounds_SecondChanceRulesId",
                table: "SeasonEventRounds",
                column: "SecondChanceRulesId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEventRounds_SecondChanceRules_SecondChanceRulesId",
                table: "SeasonEventRounds",
                column: "SecondChanceRulesId",
                principalTable: "SecondChanceRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
