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

public class LoginManagerQueryValidator : AbstractValidator<LoginManagerQuery>
{
    public LoginManagerQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
