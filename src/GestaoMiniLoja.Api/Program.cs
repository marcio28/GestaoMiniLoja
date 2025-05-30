using GestaoMiniLoja.Api.Configuration;
using GestaoMiniLoja.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiConfig()
       .AddSwaggerConfig()
       .AddDatabaseSelector()
       .AddIdentityAndJwtConfig();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

DbMigrationHelpers.EnsureSeedData(app).Wait();

app.Run();
