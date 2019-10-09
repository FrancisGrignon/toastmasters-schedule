using Microsoft.EntityFrameworkCore.Migrations;

namespace Members.DataAccess.Migrations
{
    public partial class TableMemberAddNotify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Notify",
                table: "Members",
                nullable: false,
                defaultValue: true,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<bool>(
                name: "Notify2",
                table: "Members",
                nullable: false,
                defaultValue: true,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<bool>(
                name: "Notify3",
                table: "Members",
                nullable: false,
                defaultValue: true,
                defaultValueSql: "1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notify",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Notify2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Notify3",
                table: "Members");
        }
    }
}
