using GestaoMiniLoja.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoMiniLoja.Data.Configurations
{
    public class ProdutoEntityTypeConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(e => e.Descricao).HasColumnType("VARCHAR(500)");
            builder.Property(e => e.CaminhoDaImagem).HasColumnType("VARCHAR(200)");
            builder.Property(e => e.Preco).HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(e => e.QuantidadeEmEstoque).HasColumnType("INT").IsRequired();
            builder.Property(e => e.CategoriaDeProdutoId).HasColumnType("INT").IsRequired();
            builder.Property(e => e.VendedorId).HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(e => e.CategoriaDeProduto).WithMany().HasForeignKey(e => e.CategoriaDeProdutoId);
        }
    }
}
