using FindMyPrompter.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FindMyPrompter.Api.Controllers;

// MapIdentityApi cobre register/login/refresh, mas não expõe logout.
[ApiController]
[Route("api/auth")]
public class AuthController(SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }
}
