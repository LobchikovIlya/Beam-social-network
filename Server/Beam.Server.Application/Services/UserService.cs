using Beam.Application.Dto;
using Beam.Application.Services.Interfaces;
using Beam.Application.Utilities;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

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
        var users = await _dbContext.Users.ToListAsync();
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Tag = user.Tag,
            Name = user.Name,
            CreationDate = user.CreationDate
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
            CreationDate = user.CreationDate
        };
    }

    public async Task<UserDto> CreateAsync(UserInputDto input)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Tag = input.Tag,
            Name = input.Name,
            PasswordHash = PasswordHasher.HashPassword(input.Password),
            CreationDate = DateTimeOffset.UtcNow
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Tag = user.Tag,
            Name = user.Name,
            CreationDate = user.CreationDate
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
            CreationDate = user.CreationDate
        };
    }
}