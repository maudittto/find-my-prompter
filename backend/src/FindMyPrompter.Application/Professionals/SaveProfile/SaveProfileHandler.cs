using FindMyPrompter.Domain.Professionals;

namespace FindMyPrompter.Application.Professionals.SaveProfile;

public sealed class SaveProfileHandler(IProfileStore store)
{
    public async Task<SaveProfileResult> HandleAsync(
        Guid userId,
        SaveProfileInput input,
        CancellationToken cancellationToken)
    {
        var username = input.Username.Trim().ToLowerInvariant();

        if (await store.IsUsernameTakenAsync(username, userId, cancellationToken))
        {
            return new SaveProfileResult(SaveProfileOutcome.UsernameTaken, Profile: null);
        }

        var profile = await store.GetByUserIdAsync(userId, cancellationToken);
        var outcome = profile is null ? SaveProfileOutcome.Created : SaveProfileOutcome.Updated;
        profile ??= new ProfessionalProfile(userId);

        profile.Update(
            username,
            input.DisplayName,
            input.Headline,
            input.About,
            input.Location,
            input.Seniority,
            input.WorkMode,
            input.Skills,
            input.AiModels,
            input.Experiences.Select(experience => new ProfessionalExperience(
                experience.Company,
                experience.Position,
                experience.Description,
                experience.Location,
                experience.StartDate,
                experience.EndDate)));

        await store.SaveAsync(profile, cancellationToken);

        return new SaveProfileResult(outcome, profile);
    }
}
