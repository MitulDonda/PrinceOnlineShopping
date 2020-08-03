using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineShoping.Services.Migrations
{
    public partial class addPaymentGatewayResponseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "paymentGatewayResponses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderAmount = table.Column<string>(nullable: true),
                    referenceId = table.Column<string>(nullable: true),
                    txStatus = table.Column<string>(nullable: true),
                    paymentMode = table.Column<string>(nullable: true),
                    txMsg = table.Column<string>(nullable: true),
                    txTime = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentGatewayResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_paymentGatewayResponses_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_paymentGatewayResponses_OrderId",
                table: "paymentGatewayResponses",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paymentGatewayResponses");
        }
    }
}
