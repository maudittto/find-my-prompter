using FindMyPrompter.Application.Professionals.SaveProfile;
using FindMyPrompter.Application.Validation;
using FindMyPrompter.Domain.Professionals;
using FluentValidation.Results;

namespace FindMyPrompter.Api.Contracts.Profiles;

/// <summary>Request de <c>PUT /api/profiles/me</c> — cria ou atualiza o próprio perfil.</summary>
public sealed record SaveProfileRequest(
    string Username,
    string DisplayName,
    string? Headline,
    string? About,
    string? Location,
    Seniority? Seniority,
    WorkMode? WorkMode,
    IReadOnlyList<string>? Skills,
    IReadOnlyList<string>? AiModels,
    IReadOnlyList<ExperienceRequest>? Experiences) : IValidatableRequest
{
    public ValidationResult Validate() =>
        new SaveProfileRequestValidator().Validate(this);

    public SaveProfileInput ToInput() =>
        new(
            Username,
            DisplayName,
            Headline,
            About,
            Location,
            Seniority,
            WorkMode,
            Skills ?? [],
            AiModels ?? [],
            [.. (Experiences ?? []).Select(experience => experience.ToInput())]);
}

public sealed record ExperienceRequest(
    string Company,
    string Position,
    string? Description,
    string? Location,
    DateOnly StartDate,
    DateOnly? EndDate)
{
    public SaveExperienceInput ToInput() =>
        new(Company, Position, Description, Location, StartDate, EndDate);
}
