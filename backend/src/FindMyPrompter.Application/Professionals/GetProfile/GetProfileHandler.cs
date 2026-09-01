using FindMyPrompter.Domain.Professionals;

namespace FindMyPrompter.Application.Professionals.GetProfile;

public sealed class GetProfileHandler(IProfileStore store)
{
    public Task<ProfessionalProfile?> ByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        store.GetByUserIdAsync(userId, cancellationToken);

    public Task<ProfessionalProfile?> ByUsernameAsync(string username, CancellationToken cancellationToken) =>
        store.GetByUsernameAsync(username.Trim().ToLowerInvariant(), cancellationToken);
}
