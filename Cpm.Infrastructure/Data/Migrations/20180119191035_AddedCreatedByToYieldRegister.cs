using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedCreatedByToYieldRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "YieldRegisters",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "YieldRegisters",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "YieldRegisters");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "YieldRegisters");
        }
    }
}
