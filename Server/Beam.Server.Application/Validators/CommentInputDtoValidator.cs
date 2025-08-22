using Beam.Shared.Dto;
using FluentValidation;

namespace Beam.Application.Validators;

public class CommentInputDtoValidator : AbstractValidator<CommentInputDto>
{
    public CommentInputDtoValidator()
    {
        RuleFor(c => c.Content)
            .NotEmpty().WithMessage("Content cannot be empty.")
            .Length(1, 1000).WithMessage("Content must be between 1 and 100 characters.");
    }
}