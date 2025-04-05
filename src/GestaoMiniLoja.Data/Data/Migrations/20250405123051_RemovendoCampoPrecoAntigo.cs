using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoMiniLoja.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovendoCampoPrecoAntigo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "Produtos");

            migrationBuilder.AlterColumn<string>(
                name: "Preco",
                table: "Produtos",
                type: "VARCHAR(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Preco",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitario",
                table: "Produtos",
                type: "DECIMAL(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
