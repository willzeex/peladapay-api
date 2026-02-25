using FluentValidation;
using PeladaPay.Application.Features.Auth.Commands;
using PeladaPay.Application.Features.Auth.Queries;

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
    private static readonly string[] ValidFrequencies = ["Semanal", "Quinzenal", "Mensal"];

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
