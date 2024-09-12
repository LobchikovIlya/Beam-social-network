﻿using Beam.Application.Services.Interfaces;
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
            throw new InvalidOperationException("User not found");
        }
        return user;
    }

    public async Task<Guid> CreateAsync(User input)
    {
        await _dbContext.Users.AddAsync(input);
        await _dbContext.SaveChangesAsync();

        return input.Id;
    }

    public async Task<Guid> UpdateAsync(Guid id,User input)
    {
        var user =await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
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
            throw new InvalidOperationException("User not found");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        
    }
    
}