using FindMyPrompter.Api.Contracts.Profiles;
using FindMyPrompter.Application.Messages;
using FindMyPrompter.Domain.Professionals;

namespace FindMyPrompter.Api.IntegrationTests;

public class SaveProfileRequestValidatorTests
{
    private readonly SaveProfileRequestValidator _sut = new();

    private SaveProfileRequest Request(string username = "ada", params ExperienceRequest[] experiences) =>
        new(
            username,
            "Ada Prompter",
            "Prompt Engineer",
            "Trabalho com LLMs.",
            "São Paulo",
            Seniority.Senior,
            WorkMode.Remote,
            ["Prompt Engineering"],
            ["Claude"],
            experiences);

    [Fact]
    public void Validate_Should_BeValid_When_RequestIsWellFormed()
    {
        // Arrange
        var request = Request();

        // Act
        var result = _sut.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_ReturnError_When_UsernameIsMissing()
    {
        // Arrange
        var request = Request(username: "");

        // Act
        var result = _sut.Validate(request);

        // Assert
        Assert.Contains(result.Errors, error => error.ErrorMessage == Messages.Profiles.UsernameRequired);
    }

    [Fact]
    public void Validate_Should_ReturnOnlyRequiredError_When_UsernameIsMissing()
    {
        // Arrange
        var request = Request(username: "   ");

        // Act
        var result = _sut.Validate(request);

        // Assert — as regras condicionais não devem disparar junto com NotEmpty.
        Assert.Equal(Messages.Profiles.UsernameRequired, Assert.Single(result.Errors).ErrorMessage);
    }

    [Theory]
    [InlineData("ab", nameof(Messages.Profiles.UsernameLength))]
    [InlineData("com espaco", nameof(Messages.Profiles.UsernameFormat))]
    [InlineData("-comeca-com-hifen", nameof(Messages.Profiles.UsernameFormat))]
    public void Validate_Should_ReturnError_When_UsernameIsMalformed(string username, string _)
    {
        // Arrange
        var request = Request(username);

        // Act
        var result = _sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("ME")]
    public void Validate_Should_ReturnError_When_UsernameIsReserved(string username)
    {
        // Arrange
        var request = Request(username);

        // Act
        var result = _sut.Validate(request);

        // Assert
        Assert.Contains(result.Errors, error => error.ErrorMessage == Messages.Profiles.UsernameReserved);
    }

    [Fact]
    public void Validate_Should_ReturnError_When_ExperienceEndsBeforeItStarts()
    {
        // Arrange
        var request = Request(
            "ada",
            new ExperienceRequest(
                "Acme",
                "Prompt Engineer",
                null,
                null,
                new DateOnly(2024, 1, 1),
                new DateOnly(2023, 1, 1)));

        // Act
        var result = _sut.Validate(request);

        // Assert
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == Messages.Profiles.ExperienceEndBeforeStart);
    }

    [Fact]
    public void Validate_Should_ReturnError_When_ExperienceCompanyIsMissing()
    {
        // Arrange
        var request = Request(
            "ada",
            new ExperienceRequest("", "Prompt Engineer", null, null, new DateOnly(2023, 1, 1), null));

        // Act
        var result = _sut.Validate(request);

        // Assert
        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage == Messages.Profiles.ExperienceCompanyRequired);
    }

    [Fact]
    public void Validate_Should_ReturnError_When_ThereAreTooManySkills()
    {
        // Arrange
        var request = Request() with { Skills = [.. Enumerable.Range(0, 31).Select(index => $"skill-{index}")] };

        // Act
        var result = _sut.Validate(request);

        // Assert
        Assert.Contains(result.Errors, error => error.ErrorMessage == Messages.Profiles.TooManySkills);
    }
}
