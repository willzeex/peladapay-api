using FluentValidation;
using PeladaPay.Application.Features.Groups.Commands;
using PeladaPay.Application.Features.Payments.Commands;
using PeladaPay.Application.Features.Payments.Webhooks;

namespace PeladaPay.Application.Validators;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MatchDate).GreaterThan(DateTime.UtcNow.Date.AddDays(-1));
        RuleFor(x => x.PixKey).NotEmpty().MaximumLength(120);
    }
}

public class GeneratePixChargeCommandValidator : AbstractValidator<GeneratePixChargeCommand>
{
    public GeneratePixChargeCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEqual(Guid.Empty);
        RuleFor(x => x.PlayerName).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class AddPlayerToGroupCommandValidator : AbstractValidator<AddPlayerToGroupCommand>
{
    public AddPlayerToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Cpf).NotEmpty().MaximumLength(14);
        RuleFor(x => x.Type)
            .IsInEnum()
            .NotEqual((PeladaPay.Domain.Enums.PlayerType)0);
        RuleFor(x => x.Email).MaximumLength(120).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
    }
}

public class ConfirmPaymentWebhookCommandValidator : AbstractValidator<ConfirmPaymentWebhookCommand>
{
    public ConfirmPaymentWebhookCommandValidator()
    {
        RuleFor(x => x.ChargeId).NotEmpty();
        RuleFor(x => x.ReceiptUrl).NotEmpty().Must(u => Uri.IsWellFormedUriString(u, UriKind.Absolute));
    }
}
