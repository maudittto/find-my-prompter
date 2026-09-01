using FindMyPrompter.Application.Messages;
using FluentValidation;

namespace FindMyPrompter.Api.Contracts.Profiles;

public sealed class ExperienceRequestValidator : AbstractValidator<ExperienceRequest>
{
    public ExperienceRequestValidator()
    {
        RuleFor(experience => experience.Company)
            .NotEmpty()
            .WithMessage(Messages.Profiles.ExperienceCompanyRequired)
            .MaximumLength(120)
            .WithMessage(Messages.Profiles.ExperienceCompanyTooLong);

        RuleFor(experience => experience.Position)
            .NotEmpty()
            .WithMessage(Messages.Profiles.ExperiencePositionRequired)
            .MaximumLength(120)
            .WithMessage(Messages.Profiles.ExperiencePositionTooLong);

        RuleFor(experience => experience.Description)
            .MaximumLength(2000)
            .WithMessage(Messages.Profiles.ExperienceDescriptionTooLong);

        RuleFor(experience => experience.Location)
            .MaximumLength(120)
            .WithMessage(Messages.Profiles.ExperienceLocationTooLong);

        RuleFor(experience => experience.StartDate)
            .NotEmpty()
            .WithMessage(Messages.Profiles.ExperienceStartDateRequired);

        RuleFor(experience => experience.EndDate)
            .Must(EndNotBeforeStart)
            .WithMessage(Messages.Profiles.ExperienceEndBeforeStart);
    }

    private bool EndNotBeforeStart(ExperienceRequest experience, DateOnly? endDate) =>
        endDate is null || endDate >= experience.StartDate;
}
