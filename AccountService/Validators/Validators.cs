using AccountService.Dtos;
using FluentValidation;

namespace AccountService.Validators;

public class CreateAccountValidator: AbstractValidator<CreateAccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Owner).NotEmpty().MaximumLength(100);
    }

}
public class CreateMovementValidator : AbstractValidator<CreateMovementRequest>
{
    public CreateMovementValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(200);
    }

}