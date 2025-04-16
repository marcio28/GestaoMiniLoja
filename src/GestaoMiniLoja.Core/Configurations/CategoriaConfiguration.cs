using GestaoMiniLoja.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoMiniLoja.Core.Configurations
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("CategoriasDeProduto");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Descricao).HasColumnType("VARCHAR(50)").IsRequired();
        }
    }
}
