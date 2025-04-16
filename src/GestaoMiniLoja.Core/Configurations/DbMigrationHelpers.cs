using GestaoMiniLoja.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GestaoMiniLoja.Core.Configurations
{
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelpers.EnsureSeedData(app).Wait();
        }
    }

    public static class DbMigrationHelpers
    {
        public static async Task EnsureSeedData(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
            {
                await context.Database.MigrateAsync();

                await EnsureSeedData(context);
            }
        }

        private static async Task EnsureSeedData(AppDbContext context)
        {
            int categoriaDeProdutoId = -1;

            if (context.Categorias.Any(c => c.Id == categoriaDeProdutoId))
                return;

            await context.Categorias.AddAsync(new Categoria()
            {
                Id = categoriaDeProdutoId,
                Descricao = "Livros"
            });

            await context.SaveChangesAsync();

            var vendedorId = Guid.NewGuid();

            if (context.Vendedores.Any(v => v.Id == vendedorId))
                return;

            await context.Vendedores.AddAsync(new Vendedor()
            {
                Id = vendedorId
            });

            int produto1Id = -1;

            if (context.Produtos.Any(p => p.Id == produto1Id))
                return;

            await context.Produtos.AddAsync(new Produto()
            {
                Id = -1,
                Nome = "Livro CSS",
                Descricao = "Aprenda CSS de forma didática, com exemplos.",
                CaminhoDaImagem = "https://m.media-amazon.com/images/I/41hoJ5QbTjL._SY445_SX342_.jpg",
                Preco = "R$ 50,00",
                Estoque = 10,
                CategoriaId = categoriaDeProdutoId,
                VendedorId = vendedorId
            });

            int produto2Id = -2;

            if (context.Produtos.Any(p => p.Id == produto2Id))
                return;

            await context.Produtos.AddAsync(new Produto()
            {
                Id = -2,
                Nome = "Livro jQuery",
                Descricao = "Aprenda jQuery em 2 dias.",
                CaminhoDaImagem = "https://m.media-amazon.com/images/I/41T51Y1sMgL._SY445_SX342_.jpg",
                Preco = "R$ 1,99",
                Estoque = 10,
                CategoriaId = categoriaDeProdutoId,
                VendedorId = vendedorId
            });

            await context.SaveChangesAsync();

            if (context.Users.Any(u => u.Id == vendedorId.ToString()))
                return;

            await context.Users.AddAsync(new IdentityUser
            {
                Id = vendedorId.ToString(),
                UserName = "teste@teste.com",
                NormalizedUserName = "TESTE@TESTE.COM",
                Email = "teste@teste.com",
                NormalizedEmail = "TESTE@TESTE.COM",
                AccessFailedCount = 0,
                LockoutEnabled = false,
                PasswordHash = "AQAAAAIAAYagAAAAEEdWhqiCwW/jZz0hEM7aNjok7IxniahnxKxxO5zsx2TvWs4ht1FUDnYofR8JKsA5UA==", // Teste@123
                TwoFactorEnabled = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            });

            await context.SaveChangesAsync();
        }
    }
}
