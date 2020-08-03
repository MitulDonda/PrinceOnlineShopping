using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineShoping.Services.Migrations
{
    public partial class AddCancelDateTimeToOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDateTime",
                table: "Orders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledDateTime",
                table: "Orders");
        }
    }
}
