using Beam.Application.Services.Interfaces;
using Beam.Application.Utilities;
using Beam.Application.Validators;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Entities;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Beam.Application.Services;

public class UserService : IUserService
{
    private readonly BeamDbContext _dbContext;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IValidator<UserInputDto> _validator;

    public UserService(BeamDbContext dbContext, IHubContext<ChatHub> hubContext, IValidator<UserInputDto> validator)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _validator = validator;
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
            IsOnline = user.IsOnline
        }).ToList();
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) throw new NotFoundException($"Пользователь с Id {id} не найден.");

        return new UserDto
        {
            Id = user.Id,
            Tag = user.Tag,
            Name = user.Name,
            CreationDate = user.CreationDate,
            IsOnline = user.IsOnline
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
                IsOnline = u.IsOnline
            })
            .ToListAsync();
    }


    public async Task<UserDto> CreateAsync(UserInputDto input)
    {
      
        var validationResult = await _validator.ValidateAsync(input);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
      

        var user = new User
        {
            Id = Guid.NewGuid(),
            Tag = input.Tag,
            Name = input.Name,
            PasswordHash = PasswordHasher.HashPassword(input.Password),
            CreationDate = DateTimeOffset.UtcNow,
            IsOnline = false
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("UserListUpdated");

        return new UserDto
        {
            Id = user.Id,
            Tag = user.Tag,
            Name = user.Name,
            CreationDate = user.CreationDate,
            IsOnline = false
        };
    }

    public async Task<Guid> UpdateAsync(Guid id, UserInputDto input)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) throw new NotFoundException($"Пользователь с Id {id} не найден.");

        user.Name = input.Name;
        user.Tag = input.Tag;


        if (!string.IsNullOrEmpty(input.Password)) user.PasswordHash = PasswordHasher.HashPassword(input.Password);

        await _dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) throw new NotFoundException($"Пользователь с Id {id} не найден.");

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }


  

   
   

   
}