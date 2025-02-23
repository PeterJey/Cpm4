using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedHarvestProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HarvestProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    ProfileName = table.Column<string>(nullable: false),
                    Quality = table.Column<string>(nullable: false),
                    SerializedCriteria = table.Column<string>(nullable: false),
                    SerializedPoints = table.Column<string>(nullable: false),
                    StartingWeek = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestProfiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HarvestProfiles");
        }
    }
}
