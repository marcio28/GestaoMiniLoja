using GestaoMiniLoja.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoMiniLoja.Web.Configurations
{
    public static class DatabaseSelectorExtension
    {
        public static void AddDatabaseSelector(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionLite")));
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), a => a.EnableRetryOnFailure(
                                                                                                                    maxRetryCount: 2,
                                                                                                                    maxRetryDelay: TimeSpan.FromSeconds(5),
                                                                                                                    errorNumbersToAdd: null)));
            }
        }
    }
}
