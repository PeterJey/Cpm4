using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedForecastStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForecastStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlgorithmName = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    FieldId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    SampleCount = table.Column<int>(nullable: false),
                    ScenarioId = table.Column<string>(nullable: true),
                    SerializedStats = table.Column<string>(nullable: true),
                    StartingWeek = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForecastStats_ScenarioId_FieldId_AlgorithmName",
                table: "ForecastStats",
                columns: new[] { "ScenarioId", "FieldId", "AlgorithmName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForecastStats");
        }
    }
}
