using FindMyPrompter.Domain.Professionals;

namespace FindMyPrompter.Api.Contracts.Profiles;

public sealed record ProfileResponse(
    string Username,
    string DisplayName,
    string? Headline,
    string? About,
    string? Location,
    Seniority? Seniority,
    WorkMode? WorkMode,
    IReadOnlyList<string> Skills,
    IReadOnlyList<string> AiModels,
    IReadOnlyList<ExperienceResponse> Experiences,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public ProfileResponse(ProfessionalProfile profile)
        : this(
            profile.Username,
            profile.DisplayName,
            profile.Headline,
            profile.About,
            profile.Location,
            profile.Seniority,
            profile.WorkMode,
            profile.Skills,
            profile.AiModels,
            [.. profile.Experiences
                .OrderByDescending(experience => experience.StartDate)
                .Select(experience => new ExperienceResponse(experience))],
            profile.CreatedAt,
            profile.UpdatedAt)
    {
    }
}

public sealed record ExperienceResponse(
    Guid Id,
    string Company,
    string Position,
    string? Description,
    string? Location,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool CurrentlyWorking)
{
    public ExperienceResponse(ProfessionalExperience experience)
        : this(
            experience.Id,
            experience.Company,
            experience.Position,
            experience.Description,
            experience.Location,
            experience.StartDate,
            experience.EndDate,
            experience.CurrentlyWorking)
    {
    }
}
