using System.ComponentModel.DataAnnotations;
using Beam.Application.Services.Interfaces;
using Beam.Application.Utilities;
using Beam.Application.Validators;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Beam.Application.Services;

public class UserService : IUserService
{
    private readonly BeamDbContext _dbContext;

    public UserService(BeamDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _dbContext.Users.AsNoTracking().ToListAsync();
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Tag = user.Tag,
            Name = user.Name,
            CreationDate = user.CreationDate,
            IsOnline = user.IsOnline,
        }).ToList();
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"Пользователь с Id {id} не найден.");
        }

        return new UserDto
        {
            Id = user.Id,
            Tag = user.Tag,
            Name = user.Name,
            CreationDate = user.CreationDate,
            IsOnline = user.IsOnline,
        };
    }
    public async Task<List<UserDto>> GetUsersByIdsAsync(List<Guid> userIds)
    {
        return await _dbContext.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserDto
            {
                Id = u.Id,
                Tag = u.Tag,
                IsOnline = u.IsOnline,
            })
            .ToListAsync();
    }


    public async Task<UserDto> CreateAsync(UserInputDto input)
    {
        var validator = new UserInputDtoValidator();
        var validationResult = await validator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Tag = input.Tag,
            Name = input.Name,
            PasswordHash = PasswordHasher.HashPassword(input.Password),
            CreationDate = DateTimeOffset.UtcNow,
            IsOnline = false,
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Tag = user.Tag,
            Name = user.Name,
            CreationDate = user.CreationDate,
            IsOnline =false
        };
    }

    public async Task<Guid> UpdateAsync(Guid id, UserInputDto input)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"Пользователь с Id {id} не найден.");
        }

        user.Name = input.Name;
        user.Tag = input.Tag;
       

        if (!string.IsNullOrEmpty(input.Password))
        {
            user.PasswordHash = PasswordHasher.HashPassword(input.Password);
        }

        await _dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"Пользователь с Id {id} не найден.");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<UserDto> ValidateUserAsync(string tag, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Tag == tag);
        if (user == null)
        {
            throw new NotFoundException("Пользователь не найден.");
        }

        bool isPasswordValid = PasswordHasher.VerifyPassword(user.PasswordHash, password);
        if (!isPasswordValid)
        {
            throw new BadRequestException("Неверный пароль.");
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Tag = user.Tag,
            CreationDate = user.CreationDate,
            IsOnline = user.IsOnline,
        };
    }

    public async Task SetOnlineStatusAsync(Guid userId, bool isOnline)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsOnline = isOnline;
            await _dbContext.SaveChangesAsync();    
        }
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if(user != null)
        {
           
            user.IsOnline = false;
            await UpdateLastActivityAsync(userId);
            await _dbContext.SaveChangesAsync();
            
            
        }
    }

    public async Task UpdateLastActivityAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastActivity = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }
}