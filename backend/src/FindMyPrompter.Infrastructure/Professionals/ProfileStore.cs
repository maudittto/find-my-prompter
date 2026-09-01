using FindMyPrompter.Application.Professionals;
using FindMyPrompter.Domain.Professionals;
using FindMyPrompter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FindMyPrompter.Infrastructure.Professionals;

public sealed class ProfileStore(AppDbContext dbContext) : IProfileStore
{
    public Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.ProfessionalProfiles
            .FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);

    public Task<ProfessionalProfile?> GetByUsernameAsync(string username, CancellationToken cancellationToken) =>
        dbContext.ProfessionalProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.Username == username, cancellationToken);

    public Task<bool> IsUsernameTakenAsync(
        string username,
        Guid excludingUserId,
        CancellationToken cancellationToken) =>
        dbContext.ProfessionalProfiles.AnyAsync(
            profile => profile.Username == username && profile.UserId != excludingUserId,
            cancellationToken);

    public async Task SaveAsync(ProfessionalProfile profile, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(profile).State == EntityState.Detached)
        {
            dbContext.Add(profile);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
