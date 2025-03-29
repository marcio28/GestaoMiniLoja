using GestaoMiniLoja.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoMiniLoja.Web.Data
{
    public class CategoriaDeProdutoEntityTypeConfiguration : IEntityTypeConfiguration<CategoriaDeProduto>
    {
        public void Configure(EntityTypeBuilder<CategoriaDeProduto> builder)
        {
            builder.ToTable("CategoriasDeProduto");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Descricao).HasColumnType("VARCHAR(50)").IsRequired();
        }
    }
}
