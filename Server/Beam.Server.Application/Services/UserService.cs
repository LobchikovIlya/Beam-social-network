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

    public async Task<List<User>> GetAllAsync()
    {
        var users = await _dbContext.Users.ToListAsync();
        return users;
    }

    public async Task<User>GetByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new InvalidOperationException($"User Id {id} not found");
        }
        return user;
    }

    public async Task<Guid> CreateAsync(User input)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Tag = input.Tag,
            Name = input.Name,
            PasswordHash = PasswordHasher.HashPassword(input.PasswordHash),
            CreationDate = DateTimeOffset.UtcNow
        };
        await _dbContext.Users.AddAsync(input);
        await _dbContext.SaveChangesAsync();

        return input.Id;
    }

    public async Task<Guid> UpdateAsync(Guid id,User input)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with Id {id} not found");
        }

        user.Name = input.Name;
        user.Tag = input.Tag;
        
        await _dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User Id {id}not found");
        }
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }
}