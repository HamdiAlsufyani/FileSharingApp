using Microsoft.EntityFrameworkCore.Migrations;

namespace FileSharingPractice.Migrations
{
    public partial class addOriginalToUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileNmae",
                table: "Uploads",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileNmae",
                table: "Uploads");
        }
    }
}
