using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedPictureMetadataToPinnedNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PinnedNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SerializedPictureMetadata",
                table: "PinnedNotes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PinnedNotes");

            migrationBuilder.DropColumn(
                name: "SerializedPictureMetadata",
                table: "PinnedNotes");
        }
    }
}
