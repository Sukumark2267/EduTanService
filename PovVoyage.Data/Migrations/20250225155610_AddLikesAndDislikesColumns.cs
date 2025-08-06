using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PovVoyage.Data.Migrations
{
    public partial class AddLikesAndDislikesColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "VideoMetadata",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "VideoMetadata",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "VideoMetadata",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "VideoMetadata");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "VideoMetadata");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "VideoMetadata");
        }
    }
}
