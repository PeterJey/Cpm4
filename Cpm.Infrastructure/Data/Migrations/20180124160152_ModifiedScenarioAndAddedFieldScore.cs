using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class ModifiedScenarioAndAddedFieldScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChillUnits",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "GrowingDegreeHours",
                table: "Fields");

            migrationBuilder.AddColumn<string>(
                name: "SerializedSettings",
                table: "Scenarios",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FieldScores",
                columns: table => new
                {
                    FieldId = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    BudgetPerHectare = table.Column<decimal>(nullable: false),
                    BudgetPerPlant = table.Column<decimal>(nullable: false),
                    ChillUnits = table.Column<decimal>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    GrowingDegreeHours = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldScores", x => new { x.FieldId, x.Version });
                    table.ForeignKey(
                        name: "FK_FieldScores_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "FieldId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldScores");

            migrationBuilder.DropColumn(
                name: "SerializedSettings",
                table: "Scenarios");

            migrationBuilder.AddColumn<decimal>(
                name: "ChillUnits",
                table: "Fields",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrowingDegreeHours",
                table: "Fields",
                nullable: true);
        }
    }
}
