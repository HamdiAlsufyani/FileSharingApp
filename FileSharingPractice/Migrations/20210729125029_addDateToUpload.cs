using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileSharingPractice.Migrations
{
    public partial class addDateToUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Uploadedate",
                table: "Uploads",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uploadedate",
                table: "Uploads");
        }
    }
}
