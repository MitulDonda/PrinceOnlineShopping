using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineShoping.Services.Migrations
{
    public partial class AddIsCancelledColToOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCancelled",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCancelled",
                table: "Orders");
        }
    }
}
