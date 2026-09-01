using FindMyPrompter.Domain.Professionals;

namespace FindMyPrompter.Application.Professionals;

// Porta de persistência do módulo Professionals. Não é um repositório genérico.
public interface IProfileStore
{
    Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<ProfessionalProfile?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<bool> IsUsernameTakenAsync(string username, Guid excludingUserId, CancellationToken cancellationToken);

    Task SaveAsync(ProfessionalProfile profile, CancellationToken cancellationToken);
}
