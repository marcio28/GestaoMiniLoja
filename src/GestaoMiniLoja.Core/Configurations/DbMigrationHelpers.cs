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
            var vendedorId = Guid.Parse("13be6992-66bc-46b2-a682-c5abca6a4d02");

            await AssegurarAmostraDeCategoria(context, categoriaDeProdutoId);
            await AssegurarAmostraDeVendedor(context, vendedorId);
            await AssegurarAmostraDeProdutos(context, categoriaDeProdutoId, vendedorId);
            await AssegurarAmostraDeUser(context, vendedorId);

            await context.SaveChangesAsync();
        }

        private static async Task AssegurarAmostraDeCategoria(AppDbContext context, int categoriaId)
        {
            if (!context.Categorias.Any(c => c.Id == categoriaId))
            {
                await context.Categorias.AddAsync(new Categoria()
                {
                    Id = categoriaId,
                    Descricao = "Livros"
                });
            }
        }

        private static async Task AssegurarAmostraDeVendedor(AppDbContext context, Guid vendedorId)
        {
            if (!context.Vendedores.Any(v => v.Id == vendedorId))
            {
                await context.Vendedores.AddAsync(new Vendedor()
                {
                    Id = vendedorId
                });
            }
        }

        private static async Task AssegurarAmostraDeProdutos(AppDbContext context, int categoriaId, Guid vendedorId)
        {
            int produto1Id = -1;
            if (!context.Produtos.Any(p => p.Id == produto1Id))
            {
                await context.Produtos.AddAsync(new Produto()
                {
                    Id = -1,
                    Nome = "Livro CSS",
                    Descricao = "Aprenda CSS de forma didática, com exemplos.",
                    CaminhoDaImagem = "https://m.media-amazon.com/images/I/41hoJ5QbTjL._SY445_SX342_.jpg",
                    Preco = "R$ 50,00",
                    Estoque = 10,
                    CategoriaId = categoriaId,
                    VendedorId = vendedorId
                });
            }

            int produto2Id = -2;
            if (!context.Produtos.Any(p => p.Id == produto2Id))
            {
                await context.Produtos.AddAsync(new Produto()
                {
                    Id = -2,
                    Nome = "Livro jQuery",
                    Descricao = "Aprenda jQuery em 2 dias.",
                    CaminhoDaImagem = "https://m.media-amazon.com/images/I/41T51Y1sMgL._SY445_SX342_.jpg",
                    Preco = "R$ 1,99",
                    Estoque = 10,
                    CategoriaId = categoriaId,
                    VendedorId = vendedorId
                });
            }
        }

        private static async Task AssegurarAmostraDeUser(AppDbContext context, Guid vendedorId)
        {
            if (!context.Users.Any(u => u.Id == vendedorId.ToString()))
            {
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
            }
        }
    }
}
