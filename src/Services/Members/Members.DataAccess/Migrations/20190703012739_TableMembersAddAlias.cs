using Microsoft.EntityFrameworkCore.Migrations;

namespace Members.DataAccess.Migrations
{
    public partial class TableMembersAddAlias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Members",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Members] SET Alias = Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Members");
        }
    }
}
