
using Beam.Shared.Dto;
using FluentValidation;

namespace Beam.Application.Validators;

public class PostInputDtoValidator : AbstractValidator<PostInputDto>
{
   public PostInputDtoValidator()
   {
      RuleFor(p =>p.Content)
         .NotEmpty().WithMessage("Content cannot be empty.")
         .Length(1, 100).WithMessage("Content must be between 1 and 100 characters.");
      
     
   }
}