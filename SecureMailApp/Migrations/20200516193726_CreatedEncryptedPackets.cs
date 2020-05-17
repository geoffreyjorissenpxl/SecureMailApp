using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecureMailApp.Migrations
{
    public partial class CreatedEncryptedPackets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EncryptedPackets",
                columns: table => new
                {
                    EncryptedPacketId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderEmail = table.Column<string>(nullable: true),
                    ReceiverEmail = table.Column<string>(nullable: true),
                    EncryptedSessionKey = table.Column<byte[]>(nullable: true),
                    EncryptedData = table.Column<byte[]>(nullable: true),
                    Iv = table.Column<byte[]>(nullable: true),
                    Hmac = table.Column<byte[]>(nullable: true),
                    Signature = table.Column<byte[]>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptedPackets", x => x.EncryptedPacketId);
                    table.ForeignKey(
                        name: "FK_EncryptedPackets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncryptedPackets_UserId",
                table: "EncryptedPackets",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncryptedPackets");
        }
    }
}
