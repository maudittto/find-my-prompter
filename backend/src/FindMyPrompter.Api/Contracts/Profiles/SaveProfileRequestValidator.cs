using FindMyPrompter.Application.Messages;
using FindMyPrompter.Domain.Professionals;
using FluentValidation;

namespace FindMyPrompter.Api.Contracts.Profiles;

public sealed class SaveProfileRequestValidator : AbstractValidator<SaveProfileRequest>
{
    private const int MaxTags = 30;
    private const int MaxExperiences = 20;
    private const string UsernamePattern = "^[A-Za-z0-9](?:[A-Za-z0-9_-]*[A-Za-z0-9])?$";

    // Rotas literais da API não podem ser capturadas como perfil público.
    private readonly HashSet<string> _reservedUsernames =
        new(StringComparer.OrdinalIgnoreCase) { "me", "api", "admin", "new", "edit" };

    public SaveProfileRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty()
            .WithMessage(Messages.Profiles.UsernameRequired)
            .Length(ProfessionalProfile.UsernameMinLength, ProfessionalProfile.UsernameMaxLength)
            .When(HasUsername, ApplyConditionTo.CurrentValidator)
            .WithMessage(Messages.Profiles.UsernameLength)
            .Matches(UsernamePattern)
            .When(HasUsername, ApplyConditionTo.CurrentValidator)
            .WithMessage(Messages.Profiles.UsernameFormat)
            .Must(NotBeReserved)
            .When(HasUsername, ApplyConditionTo.CurrentValidator)
            .WithMessage(Messages.Profiles.UsernameReserved);

        RuleFor(request => request.DisplayName)
            .NotEmpty()
            .WithMessage(Messages.Profiles.DisplayNameRequired)
            .MaximumLength(80)
            .WithMessage(Messages.Profiles.DisplayNameTooLong);

        RuleFor(request => request.Headline)
            .MaximumLength(160)
            .WithMessage(Messages.Profiles.HeadlineTooLong);

        RuleFor(request => request.About)
            .MaximumLength(4000)
            .WithMessage(Messages.Profiles.AboutTooLong);

        RuleFor(request => request.Location)
            .MaximumLength(120)
            .WithMessage(Messages.Profiles.LocationTooLong);

        RuleFor(request => request.Skills)
            .Must(skills => FitsIn(skills, MaxTags))
            .WithMessage(Messages.Profiles.TooManySkills);

        RuleFor(request => request.AiModels)
            .Must(models => FitsIn(models, MaxTags))
            .WithMessage(Messages.Profiles.TooManyAiModels);

        RuleFor(request => request.Experiences)
            .Must(experiences => FitsIn(experiences, MaxExperiences))
            .WithMessage(Messages.Profiles.TooManyExperiences);

        RuleForEach(request => request.Experiences)
            .SetValidator(new ExperienceRequestValidator());
    }

    private bool HasUsername(SaveProfileRequest request) =>
        !string.IsNullOrWhiteSpace(request.Username);

    private bool NotBeReserved(string username) =>
        !_reservedUsernames.Contains(username.Trim());

    private bool FitsIn<T>(IReadOnlyList<T>? items, int limit) =>
        items is null || items.Count <= limit;
}
