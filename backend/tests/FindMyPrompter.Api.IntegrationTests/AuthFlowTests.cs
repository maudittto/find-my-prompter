using System.Net;
using System.Net.Http.Json;

namespace FindMyPrompter.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class AuthFlowTests(ApiFactory factory)
{
    private sealed record MeResponse(bool Authenticated, string? Name);

    [Fact]
    public async Task Register_login_me_and_logout()
    {
        var client = factory.CreateClient();
        var email = $"user-{Guid.NewGuid():N}@exemplo.com";
        var credentials = new { email, password = "Senha@123" };

        var register = await client.PostAsJsonAsync("/api/auth/register", credentials);
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);

        var anonymous = await client.GetAsync("/api/me");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        var login = await client.PostAsJsonAsync("/api/auth/login?useCookies=true", credentials);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var me = await client.GetFromJsonAsync<MeResponse>("/api/me");
        Assert.True(me!.Authenticated);
        Assert.Equal(email, me.Name);

        var logout = await client.PostAsync("/api/auth/logout", content: null);
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        var afterLogout = await client.GetAsync("/api/me");
        Assert.Equal(HttpStatusCode.Unauthorized, afterLogout.StatusCode);
    }

    [Fact]
    public async Task Login_with_wrong_password_is_rejected()
    {
        var client = factory.CreateClient();
        var email = $"user-{Guid.NewGuid():N}@exemplo.com";

        var register = await client.PostAsJsonAsync(
            "/api/auth/register",
            new { email, password = "Senha@123" });
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);

        var login = await client.PostAsJsonAsync(
            "/api/auth/login?useCookies=true",
            new { email, password = "SenhaErrada@123" });

        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }
}
