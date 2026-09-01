using FindMyPrompter.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FindMyPrompter.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var databaseAvailable = await dbContext.Database.CanConnectAsync(cancellationToken);

        return Ok(new
        {
            status = databaseAvailable ? "healthy" : "unhealthy",
            database = databaseAvailable ? "connected" : "disconnected"
        });
    }
}
