using Microsoft.EntityFrameworkCore.Migrations;

namespace SecureMailApp.Migrations
{
    public partial class UserRsaKeysSetAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RsaKeysSet",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RsaKeysSet",
                table: "AspNetUsers");
        }
    }
}
