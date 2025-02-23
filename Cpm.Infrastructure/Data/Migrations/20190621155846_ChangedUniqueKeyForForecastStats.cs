using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class ChangedUniqueKeyForForecastStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ForecastStats_ScenarioId_FieldId_AlgorithmName",
                table: "ForecastStats");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastStats_AlgorithmName_FieldId_ScenarioId",
                table: "ForecastStats",
                columns: new[] { "AlgorithmName", "FieldId", "ScenarioId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ForecastStats_AlgorithmName_FieldId_ScenarioId",
                table: "ForecastStats");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastStats_ScenarioId_FieldId_AlgorithmName",
                table: "ForecastStats",
                columns: new[] { "ScenarioId", "FieldId", "AlgorithmName" },
                unique: true);
        }
    }
}
