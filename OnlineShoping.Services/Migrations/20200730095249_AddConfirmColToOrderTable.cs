using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineShoping.Services.Migrations
{
    public partial class AddConfirmColToOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isConfirm",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isConfirm",
                table: "Orders");
        }
    }
}
