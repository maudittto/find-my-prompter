using System.Text.Json.Serialization;
using FindMyPrompter.Api.Filters;
using FindMyPrompter.Application.Messages;
using FindMyPrompter.Application.Professionals;
using FindMyPrompter.Application.Professionals.GetProfile;
using FindMyPrompter.Application.Professionals.SaveProfile;
using FindMyPrompter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FindMyPrompter.Infrastructure.Identity;
using FindMyPrompter.Infrastructure.Professionals;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "Connection string 'Database' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Enums trafegam como texto ("Senior") em vez de índice numérico.
builder.Services
    .AddControllers(options => options.Filters.Add<ValidationActionFilter>())
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Erro de model binding sai no mesmo formato do ValidationActionFilter.
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error =>
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["propertyName"] = entry.Key,
                    ["errorMessage"] = error.ErrorMessage
                }))
            .ToList<object?>();

        return new BadRequestObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = Messages.Validation.OneOrMoreErrorsOccurred,
            Type = "about:blank",
            Extensions = { ["errors"] = errors }
        });
    });
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IProfileStore, ProfileStore>();
builder.Services.AddScoped<SaveProfileHandler>();
builder.Services.AddScoped<GetProfileHandler>();

var app = builder.Build();

// Exceções não tratadas viram ProblemDetails em vez de vazar stack trace.
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Em desenvolvimento o front chama a API pelo proxy do Next (http).
// Redirecionar para https aqui jogaria o browser para outra origem => erro de CORS.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.MapGroup("/api/auth")
    .MapIdentityApi<ApplicationUser>();

app.Run();

public partial class Program;
