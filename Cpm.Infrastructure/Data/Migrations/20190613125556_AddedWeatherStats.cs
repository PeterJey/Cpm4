using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedWeatherStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<string>(nullable: false),
                    SampleCount = table.Column<int>(nullable: false),
                    TempMax = table.Column<decimal>(type: "DECIMAL(16,2)", nullable: false),
                    TempMin = table.Column<decimal>(type: "DECIMAL(16,2)", nullable: false),
                    When = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherStats_When_Location",
                table: "WeatherStats",
                columns: new[] { "When", "Location" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherStats");
        }
    }
}
