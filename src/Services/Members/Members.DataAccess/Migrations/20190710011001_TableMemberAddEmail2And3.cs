using Microsoft.EntityFrameworkCore.Migrations;

namespace Members.DataAccess.Migrations
{
    public partial class TableMemberAddEmail2And3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email2",
                table: "Members",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email3",
                table: "Members",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Email3",
                table: "Members");
        }
    }
}
