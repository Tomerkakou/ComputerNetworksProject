using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerNetworksProject.Migrations
{
    /// <inheritdoc />
    public partial class payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Payments");
        }
    }
}
