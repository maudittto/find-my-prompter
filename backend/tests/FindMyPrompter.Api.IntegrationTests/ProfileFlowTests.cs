using System.Net;
using System.Net.Http.Json;

namespace FindMyPrompter.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class ProfileFlowTests(ApiFactory factory)
{
    private sealed record ExperiencePayload(
        string Company,
        string Position,
        string? Description,
        string? Location,
        DateOnly StartDate,
        DateOnly? EndDate);

    private sealed record ProfilePayload(
        string Username,
        string DisplayName,
        string? Headline,
        string? About,
        string? Location,
        string? Seniority,
        string? WorkMode,
        string[] Skills,
        string[] AiModels,
        ExperiencePayload[] Experiences);

    private sealed record ExperienceView(
        Guid Id,
        string Company,
        string Position,
        DateOnly StartDate,
        DateOnly? EndDate,
        bool CurrentlyWorking);

    private sealed record ProfileView(
        string Username,
        string DisplayName,
        string? Headline,
        string? Seniority,
        string? WorkMode,
        string[] Skills,
        string[] AiModels,
        ExperienceView[] Experiences);

    private ProfilePayload ValidPayload(string username) =>
        new(
            username,
            "Ada Prompter",
            "Prompt Engineer",
            "Trabalho com LLMs.",
            "São Paulo",
            "Senior",
            "Remote",
            ["Prompt Engineering", "RAG"],
            ["Claude", "GPT"],
            [
                new ExperiencePayload(
                    "Acme",
                    "Prompt Engineer",
                    "Agentes e avaliação.",
                    "Remoto",
                    new DateOnly(2023, 1, 1),
                    null)
            ]);

    private async Task<HttpClient> AuthenticatedClient()
    {
        var client = factory.CreateClient();
        var credentials = new { email = $"user-{Guid.NewGuid():N}@exemplo.com", password = "Senha@123" };

        var register = await client.PostAsJsonAsync("/api/auth/register", credentials);
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);

        var login = await client.PostAsJsonAsync("/api/auth/login?useCookies=true", credentials);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        return client;
    }

    [Fact]
    public async Task Create_then_update_profile_and_read_it_publicly()
    {
        var client = await AuthenticatedClient();
        var username = $"ada{Guid.NewGuid():N}"[..20];

        var missing = await client.GetAsync("/api/profiles/me");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);

        // Username em caixa alta é normalizado para minúsculo.
        var created = await client.PutAsJsonAsync(
            "/api/profiles/me",
            ValidPayload(username.ToUpperInvariant()));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var createdProfile = (await created.Content.ReadFromJsonAsync<ProfileView>())!;
        Assert.Equal(username, createdProfile.Username);
        Assert.Equal("Senior", createdProfile.Seniority);
        Assert.Equal("Remote", createdProfile.WorkMode);
        Assert.Equal(["Prompt Engineering", "RAG"], createdProfile.Skills);
        Assert.True(Assert.Single(createdProfile.Experiences).CurrentlyWorking);

        var updated = await client.PutAsJsonAsync(
            "/api/profiles/me",
            ValidPayload(username) with { Headline = "Staff Prompt Engineer", Experiences = [] });
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);

        var updatedProfile = (await updated.Content.ReadFromJsonAsync<ProfileView>())!;
        Assert.Equal("Staff Prompt Engineer", updatedProfile.Headline);
        Assert.Empty(updatedProfile.Experiences);

        // Perfil público não exige sessão.
        var publicProfile = await factory.CreateClient()
            .GetFromJsonAsync<ProfileView>($"/api/profiles/{username}");
        Assert.Equal("Ada Prompter", publicProfile!.DisplayName);
    }

    [Fact]
    public async Task Username_already_used_by_another_profile_is_rejected()
    {
        var username = $"dup{Guid.NewGuid():N}"[..20];

        var owner = await AuthenticatedClient();
        var created = await owner.PutAsJsonAsync("/api/profiles/me", ValidPayload(username));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var other = await AuthenticatedClient();
        var conflict = await other.PutAsJsonAsync("/api/profiles/me", ValidPayload(username));
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
    }

    [Fact]
    public async Task Anonymous_cannot_save_a_profile()
    {
        var response = await factory.CreateClient()
            .PutAsJsonAsync("/api/profiles/me", ValidPayload($"anon{Guid.NewGuid():N}"[..20]));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("com espaco")]
    [InlineData("-comeca-com-hifen")]
    public async Task Invalid_username_is_rejected(string username)
    {
        var client = await AuthenticatedClient();

        var response = await client.PutAsJsonAsync("/api/profiles/me", ValidPayload(username));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Reserved_username_is_rejected()
    {
        var client = await AuthenticatedClient();

        var response = await client.PutAsJsonAsync("/api/profiles/me", ValidPayload("admin"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Experience_ending_before_it_starts_is_rejected()
    {
        var client = await AuthenticatedClient();
        var payload = ValidPayload($"exp{Guid.NewGuid():N}"[..20]) with
        {
            Experiences =
            [
                new ExperiencePayload(
                    "Acme",
                    "Prompt Engineer",
                    null,
                    null,
                    new DateOnly(2024, 1, 1),
                    new DateOnly(2023, 1, 1))
            ]
        };

        var response = await client.PutAsJsonAsync("/api/profiles/me", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_username_returns_not_found()
    {
        var response = await factory.CreateClient().GetAsync("/api/profiles/naoexiste123");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
