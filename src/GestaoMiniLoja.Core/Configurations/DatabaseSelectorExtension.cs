using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GestaoMiniLoja.Core.Configurations
{
    public static class DatabaseSelectorExtension
    {
        public static WebApplicationBuilder AddDatabaseSelector(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionLite")));
            }
            else
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), a => a.EnableRetryOnFailure(
                                                                                                                    maxRetryCount: 2,
                                                                                                                    maxRetryDelay: TimeSpan.FromSeconds(5),
                                                                                                                    errorNumbersToAdd: null)));
            }
            return builder;
        }
    }
}
