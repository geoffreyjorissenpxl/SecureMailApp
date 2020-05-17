using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecureMailApp.Migrations
{
    public partial class createdDateTimePropertyToEncryptedPacket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncryptedPackets_AspNetUsers_UserId",
                table: "EncryptedPackets");

            migrationBuilder.DropIndex(
                name: "IX_EncryptedPackets_UserId",
                table: "EncryptedPackets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EncryptedPackets");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiveDate",
                table: "EncryptedPackets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiveDate",
                table: "EncryptedPackets");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EncryptedPackets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncryptedPackets_UserId",
                table: "EncryptedPackets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EncryptedPackets_AspNetUsers_UserId",
                table: "EncryptedPackets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
