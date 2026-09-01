using FindMyPrompter.Domain.Professionals;

namespace FindMyPrompter.Application.Professionals.SaveProfile;

public sealed record SaveProfileInput(
    string Username,
    string DisplayName,
    string? Headline,
    string? About,
    string? Location,
    Seniority? Seniority,
    WorkMode? WorkMode,
    IReadOnlyList<string> Skills,
    IReadOnlyList<string> AiModels,
    IReadOnlyList<SaveExperienceInput> Experiences);

public sealed record SaveExperienceInput(
    string Company,
    string Position,
    string? Description,
    string? Location,
    DateOnly StartDate,
    DateOnly? EndDate);

public enum SaveProfileOutcome
{
    Created,
    Updated,
    UsernameTaken
}

public sealed record SaveProfileResult(SaveProfileOutcome Outcome, ProfessionalProfile? Profile);
