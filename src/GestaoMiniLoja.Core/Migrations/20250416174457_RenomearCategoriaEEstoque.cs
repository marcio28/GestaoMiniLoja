using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoMiniLoja.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenomearCategoriaEEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaDeProdutoId",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "QuantidadeEmEstoque",
                table: "Produtos",
                newName: "Estoque");

            migrationBuilder.RenameColumn(
                name: "CategoriaDeProdutoId",
                table: "Produtos",
                newName: "CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_CategoriaDeProdutoId",
                table: "Produtos",
                newName: "IX_Produtos_CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "CategoriasDeProduto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaId",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "Estoque",
                table: "Produtos",
                newName: "QuantidadeEmEstoque");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Produtos",
                newName: "CategoriaDeProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                newName: "IX_Produtos_CategoriaDeProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_CategoriasDeProduto_CategoriaDeProdutoId",
                table: "Produtos",
                column: "CategoriaDeProdutoId",
                principalTable: "CategoriasDeProduto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
