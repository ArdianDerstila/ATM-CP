using Microsoft.EntityFrameworkCore.Migrations;

namespace BankAtmMVC.Migrations
{
    public partial class Atm1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Kart_1000",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Kart_2000",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Kart_500",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Kart_5000",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "Mesatarja",
                table: "Transactions",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kart_1000",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Kart_2000",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Kart_500",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Kart_5000",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Mesatarja",
                table: "Transactions");
        }
    }
}
