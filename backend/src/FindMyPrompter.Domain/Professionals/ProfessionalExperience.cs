namespace FindMyPrompter.Domain.Professionals;

public sealed class ProfessionalExperience
{
    // Usado pelo EF na materialização.
    private ProfessionalExperience()
    {
    }

    public ProfessionalExperience(
        string company,
        string position,
        string? description,
        string? location,
        DateOnly startDate,
        DateOnly? endDate)
    {
        if (string.IsNullOrWhiteSpace(company))
        {
            throw new ArgumentException("Empresa é obrigatória.", nameof(company));
        }

        if (string.IsNullOrWhiteSpace(position))
        {
            throw new ArgumentException("Cargo é obrigatório.", nameof(position));
        }

        if (endDate is not null && endDate < startDate)
        {
            throw new ArgumentException(
                "Data de término não pode ser anterior à data de início.",
                nameof(endDate));
        }

        Id = Guid.CreateVersion7();
        Company = company.Trim();
        Position = position.Trim();
        Description = Normalize(description);
        Location = Normalize(location);
        StartDate = startDate;
        EndDate = endDate;
    }

    public Guid Id { get; private set; }

    public string Company { get; private set; } = null!;

    public string Position { get; private set; } = null!;

    public string? Description { get; private set; }

    public string? Location { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly? EndDate { get; private set; }

    // Derivado de EndDate para não existir estado contraditório persistido.
    public bool CurrentlyWorking => EndDate is null;

    private string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
