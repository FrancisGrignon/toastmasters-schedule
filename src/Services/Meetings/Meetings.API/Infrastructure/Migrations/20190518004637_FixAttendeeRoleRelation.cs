using Microsoft.EntityFrameworkCore.Migrations;

namespace Meetings.API.Infrastructure.Migrations
{
    public partial class FixAttendeeRoleRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendees_Roles_MeetingId",
                table: "Attendees");

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_RoleId",
                table: "Attendees",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendees_Roles_RoleId",
                table: "Attendees",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendees_Roles_RoleId",
                table: "Attendees");

            migrationBuilder.DropIndex(
                name: "IX_Attendees_RoleId",
                table: "Attendees");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendees_Roles_MeetingId",
                table: "Attendees",
                column: "MeetingId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
