using Microsoft.EntityFrameworkCore.Migrations;

namespace Meetings.API.Infrastructure.Migrations
{
    public partial class AddMeetingCancelled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cancelled",
                table: "Meetings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancelled",
                table: "Meetings");
        }
    }
}
