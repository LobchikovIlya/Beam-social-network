using Beam.Shared.Dto;
using FluentValidation;

namespace Beam.Application.Validators;

public class UserInputDtoValidator : AbstractValidator<UserInputDto>
{
    public UserInputDtoValidator()
    {
        RuleFor(u => u.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .Length(3, 20).WithMessage("Tag must be between 3 and 20 characters.")
            .Matches(@"^[A-Z]").WithMessage("Tag must start with a capital letter.");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 20).WithMessage("Name must be between 3 and 20 characters.")
            .Matches(@"^[A-Z]").WithMessage("Name must start with a capital letter.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 20).WithMessage("Password must be between 6 and 20 characters.")
            .Matches(@"^(?=.*[A-Z])").WithMessage("Password must start with a capital letter.");
    }
}