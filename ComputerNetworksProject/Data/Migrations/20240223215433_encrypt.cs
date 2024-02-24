using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerNetworksProject.Data.Migrations
{
    public partial class encrypt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "CreditCardNumberEncrypt",
                table: "Payments",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditCardNumberEncrypt",
                table: "Payments");
        }
    }
}
