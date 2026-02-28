using FluentValidation;
using PeladaPay.Application.Features.Auth.Commands;
using PeladaPay.Application.Features.Auth.Queries;
using PeladaPay.Application.Features.Users.Commands;

namespace PeladaPay.Application.Validators;

public class RegisterManagerCommandValidator : AbstractValidator<RegisterManagerCommand>
{
    public RegisterManagerCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class StartOnboardingProfileCommandValidator : AbstractValidator<StartOnboardingProfileCommand>
{
    public StartOnboardingProfileCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.Whatsapp).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class CompleteOnboardingComplianceCommandValidator : AbstractValidator<CompleteOnboardingComplianceCommand>
{
    public CompleteOnboardingComplianceCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEqual(Guid.Empty);
        RuleFor(x => x.Cpf)
            .NotEmpty()
            .Length(11, 14)
            .Matches("^[0-9.-]+$");
        RuleFor(x => x.BirthDate).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.Address).NotEmpty().MaximumLength(200);
    }
}

public class CompleteOnboardingGroupCommandValidator : AbstractValidator<CompleteOnboardingGroupCommand>
{
    private static readonly string[] ValidFrequencies = ["semanal", "Quinzenal", "Mensal"];

    public CompleteOnboardingGroupCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEqual(Guid.Empty);
        RuleFor(x => x.GroupName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Frequency).NotEmpty().Must(f => ValidFrequencies.Contains(f));
        RuleFor(x => x.Venue).MaximumLength(120);
        RuleFor(x => x.CrestUrl).MaximumLength(500);
    }
}

public class CompleteOnboardingFinancialCommandValidator : AbstractValidator<CompleteOnboardingFinancialCommand>
{
    public CompleteOnboardingFinancialCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEqual(Guid.Empty);
        RuleFor(x => x.MonthlyFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SingleMatchFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DueDay).InclusiveBetween(1, 31);
        RuleFor(x => x.IsExpenseManagementOnly).Equal(true)
            .WithMessage("É necessário declarar que a conta será usada apenas para gestão de despesas coletivas.");
    }
}

public class LoginManagerQueryValidator : AbstractValidator<LoginManagerQuery>
{
    public LoginManagerQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}


public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FullName).MaximumLength(120).When(x => x.FullName is not null);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(120).When(x => x.Email is not null);
        RuleFor(x => x.Whatsapp).MaximumLength(20).When(x => x.Whatsapp is not null);
        RuleFor(x => x.Cpf)
            .Length(11, 14)
            .Matches("^[0-9.-]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Cpf));
        RuleFor(x => x.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.BirthDate.HasValue);
        RuleFor(x => x.Address).MaximumLength(200).When(x => x.Address is not null);
    }
}

public class UpdateUserOnboardingSettingsCommandValidator : AbstractValidator<UpdateUserOnboardingSettingsCommand>
{
    private static readonly string[] ValidFrequencies = ["semanal", "Quinzenal", "Mensal"];

    public UpdateUserOnboardingSettingsCommandValidator()
    {
        RuleFor(x => x.OnboardingGroupName).MaximumLength(100).When(x => x.OnboardingGroupName is not null);
        RuleFor(x => x.OnboardingFrequency)
            .Must(f => string.IsNullOrWhiteSpace(f) || ValidFrequencies.Contains(f))
            .When(x => x.OnboardingFrequency is not null);
        RuleFor(x => x.OnboardingVenue).MaximumLength(120).When(x => x.OnboardingVenue is not null);
        RuleFor(x => x.OnboardingCrestUrl).MaximumLength(500).When(x => x.OnboardingCrestUrl is not null);
    }
}
