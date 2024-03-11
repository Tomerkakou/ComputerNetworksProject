using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerNetworksProject.Migrations
{
    /// <inheritdoc />
    public partial class cartbuynow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BuyNow",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyNow",
                table: "Carts");
        }
    }
}
