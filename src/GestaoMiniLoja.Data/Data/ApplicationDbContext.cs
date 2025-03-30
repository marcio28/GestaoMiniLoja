using GestaoMiniLoja.Data.Configurations;
using GestaoMiniLoja.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Data.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<CategoriaDeProduto> CategoriasDeProduto { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new VendedorEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoriaDeProdutoEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProdutoEntityTypeConfiguration());
        }
    }
}
