using FindMyPrompter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "Connection string 'Database' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/health", async (AppDbContext dbContext) =>
{
    var databaseAvailable = await dbContext.Database.CanConnectAsync();

    return Results.Ok(new
    {
        status = databaseAvailable ? "healthy" : "unhealthy",
        database = databaseAvailable ? "connected" : "disconnected"
    });
});

app.Run();
