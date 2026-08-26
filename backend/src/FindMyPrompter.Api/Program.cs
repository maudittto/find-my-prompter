using FindMyPrompter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FindMyPrompter.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "Connection string 'Database' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGroup("/api/auth")
    .MapIdentityApi<ApplicationUser>();

app.MapGet("/api/health", async (AppDbContext dbContext) =>
{
    var databaseAvailable = await dbContext.Database.CanConnectAsync();

    return Results.Ok(new
    {
        status = databaseAvailable ? "healthy" : "unhealthy",
        database = databaseAvailable ? "connected" : "disconnected"
    });
});

app.MapGet("/api/me", (HttpContext context) =>
{
    return Results.Ok(new
    {
        authenticated = context.User.Identity?.IsAuthenticated,
        name = context.User.Identity?.Name
    });
})
.RequireAuthorization();

app.Run();
