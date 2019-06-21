using Microsoft.EntityFrameworkCore.Migrations;

namespace Members.DataAccess.Migrations
{
    public partial class TableMemberAddDeletedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Members",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Members");
        }
    }
}
