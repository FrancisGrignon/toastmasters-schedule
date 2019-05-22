using Microsoft.EntityFrameworkCore.Migrations;

namespace Members.DataAccess.Migrations
{
    public partial class TableMembersAddToastmastersId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rank",
                table: "Members",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToastmastersId",
                table: "Members",
                nullable: false,    
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ToastmastersId",
                table: "Members");
        }
    }
}
