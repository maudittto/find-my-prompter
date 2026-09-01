using System.Security.Claims;
using FindMyPrompter.Api.Contracts.Profiles;
using FindMyPrompter.Application.Professionals.GetProfile;
using FindMyPrompter.Application.Professionals.SaveProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindMyPrompter.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileResponse>> GetMine(
        [FromServices] GetProfileHandler handler,
        CancellationToken cancellationToken)
    {
        var profile = await handler.ByUserIdAsync(CurrentUserId(), cancellationToken);

        return profile is null ? NotFound() : Ok(new ProfileResponse(profile));
    }

    [HttpGet("{username}")]
    [AllowAnonymous]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileResponse>> GetByUsername(
        string username,
        [FromServices] GetProfileHandler handler,
        CancellationToken cancellationToken)
    {
        var profile = await handler.ByUsernameAsync(username, cancellationToken);

        return profile is null ? NotFound() : Ok(new ProfileResponse(profile));
    }


    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProfileResponse>> SaveMine(
        SaveProfileRequest request,
        [FromServices] SaveProfileHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(CurrentUserId(), request.ToInput(), cancellationToken);

        if (result.Outcome == SaveProfileOutcome.UsernameTaken)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Username indisponível.",
                Detail = "Este username já está em uso por outro perfil.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var response = new ProfileResponse(result.Profile!);

        return result.Outcome == SaveProfileOutcome.Created
            ? CreatedAtAction(nameof(GetByUsername), new { username = response.Username }, response)
            : Ok(response);
    }

    private Guid CurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
