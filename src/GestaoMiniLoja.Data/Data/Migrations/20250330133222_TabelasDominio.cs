using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoMiniLoja.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelasDominio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasDeProduto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasDeProduto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Descricao = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    CaminhoDaImagem = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    PrecoUnitario = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    QuantidadeEmEstoque = table.Column<int>(type: "INT", nullable: false),
                    CategoriaDeProdutoId = table.Column<int>(type: "INT", nullable: false),
                    VendedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_CategoriasDeProduto_CategoriaDeProdutoId",
                        column: x => x.CategoriaDeProdutoId,
                        principalTable: "CategoriasDeProduto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produtos_Vendedores_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Vendedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaDeProdutoId",
                table: "Produtos",
                column: "CategoriaDeProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_VendedorId",
                table: "Produtos",
                column: "VendedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "CategoriasDeProduto");

            migrationBuilder.DropTable(
                name: "Vendedores");
        }
    }
}
