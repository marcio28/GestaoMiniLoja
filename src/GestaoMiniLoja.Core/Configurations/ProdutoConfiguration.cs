using GestaoMiniLoja.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoMiniLoja.Core.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(e => e.Descricao).HasColumnType("VARCHAR(500)");
            builder.Property(e => e.CaminhoDaImagem).HasColumnType("VARCHAR(200)");
            builder.Property(e => e.Preco).HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(e => e.Estoque).HasColumnType("INT").IsRequired();
            builder.Property(e => e.CategoriaId).HasColumnType("INT").IsRequired();
            builder.Property(e => e.VendedorId).HasColumnType("uniqueidentifier").IsRequired();
            builder.HasOne(e => e.Categoria).WithMany().HasForeignKey(e => e.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
