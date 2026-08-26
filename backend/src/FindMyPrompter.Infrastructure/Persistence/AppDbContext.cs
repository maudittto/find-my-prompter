using Microsoft.EntityFrameworkCore;

namespace FindMyPrompter.Infrastructure.Persistence;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
}