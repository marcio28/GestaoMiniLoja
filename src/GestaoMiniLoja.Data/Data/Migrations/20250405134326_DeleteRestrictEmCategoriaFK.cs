using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoMiniLoja.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteRestrictEmCategoriaFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaDeProdutoId",
                table: "Produtos");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaDeProdutoId",
                table: "Produtos",
                column: "CategoriaDeProdutoId",
                principalTable: "CategoriasDeProduto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaDeProdutoId",
                table: "Produtos");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaDeProdutoId",
                table: "Produtos",
                column: "CategoriaDeProdutoId",
                principalTable: "CategoriasDeProduto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
