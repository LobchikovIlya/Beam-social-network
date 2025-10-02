using System.Formats.Asn1;
using Beam.Infrastructure;
using Beam.Shared.Dto;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Validators;

public class UserInputDtoValidator : AbstractValidator<UserInputDto>
{
    private readonly BeamDbContext _dbContext;
    public UserInputDtoValidator(BeamDbContext dbContext)
    {
        _dbContext = dbContext;
        RuleFor(u => u.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .Length(3, 20).WithMessage("Tag must be between 3 and 20 characters.")
            .Matches(@"^[A-Z]").WithMessage("Tag must start with a capital letter.")
            .MustAsync(BeUniqueTagAsync).WithMessage("Пользователь с таким тегом существует! ");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 20).WithMessage("Name must be between 3 and 20 characters.")
            .Matches(@"^[A-Z]").WithMessage("Name must start with a capital letter.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 20).WithMessage("Password must be between 6 and 20 characters.")
            .Matches(@"^(?=.*[A-Z])").WithMessage("Password must start with a capital letter.");
    }

    private async Task<bool> BeUniqueTagAsync(string tag, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(tag))
            return true;
        return !await _dbContext.Users.AnyAsync(u => u.Tag.ToLower() == tag.ToLower(), cancellationToken);
    }
}