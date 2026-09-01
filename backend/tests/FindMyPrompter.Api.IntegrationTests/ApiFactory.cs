using FindMyPrompter.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FindMyPrompter.Api.IntegrationTests;

// Todas as classes de teste compartilham a mesma instância: o banco é recriado
// uma vez por execução e nenhuma delas o derruba enquanto outra o usa.
[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "api";
}

// Usa o Postgres do docker-compose em um banco separado dos dados de desenvolvimento.
public class ApiFactory : WebApplicationFactory<Program>
{
    private const string LocalConnectionString =
        "Host=localhost;Port=5432;Database=findmyprompter_tests;Username=findmyprompter;Password=findmyprompter";

    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("ConnectionStrings__Database") ?? LocalConnectionString;

    public ApiFactory()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Database", ConnectionString);
    }
}
