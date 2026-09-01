namespace FindMyPrompter.Domain.Professionals;

public sealed class ProfessionalProfile
{
    public const int UsernameMinLength = 3;
    public const int UsernameMaxLength = 30;

    private readonly List<ProfessionalExperience> _experiences = [];

    // Usado pelo EF na materialização.
    private ProfessionalProfile()
    {
    }

    public ProfessionalProfile(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId é obrigatório.", nameof(userId));
        }

        var now = DateTimeOffset.UtcNow;

        Id = Guid.CreateVersion7();
        UserId = userId;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Username { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;

    public string? Headline { get; private set; }

    public string? About { get; private set; }

    public string? Location { get; private set; }

    public Seniority? Seniority { get; private set; }

    public WorkMode? WorkMode { get; private set; }

    public IReadOnlyList<string> Skills { get; private set; } = [];

    public IReadOnlyList<string> AiModels { get; private set; } = [];

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyList<ProfessionalExperience> Experiences => _experiences;

    public void Update(
        string username,
        string displayName,
        string? headline,
        string? about,
        string? location,
        Seniority? seniority,
        WorkMode? workMode,
        IEnumerable<string> skills,
        IEnumerable<string> aiModels,
        IEnumerable<ProfessionalExperience> experiences)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username é obrigatório.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Nome de exibição é obrigatório.", nameof(displayName));
        }

        // O username público é sempre minúsculo: a URL do perfil não é case-sensitive.
        Username = username.Trim().ToLowerInvariant();
        DisplayName = displayName.Trim();
        Headline = NormalizeText(headline);
        About = NormalizeText(about);
        Location = NormalizeText(location);
        Seniority = seniority;
        WorkMode = workMode;
        Skills = NormalizeTags(skills);
        AiModels = NormalizeTags(aiModels);

        _experiences.Clear();
        _experiences.AddRange(experiences);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private string? NormalizeText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private List<string> NormalizeTags(IEnumerable<string> values) =>
    [
        .. values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .DistinctBy(value => value.ToLowerInvariant())
    ];
}
