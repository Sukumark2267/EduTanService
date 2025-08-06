using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PovVoyage.Data.Migrations
{
    public partial class AddThumbnailUrlToVideoMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "VideoMetadata",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "VideoMetadata");
        }
    }
}
