using Beam.Application.Services.Interfaces;
using Beam.Application.Utilities;
using Beam.Core.Exceptions;
using Beam.Infrastructure;
using Beam.Infrastructure.Hubs;
using Beam.Shared.Dto;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Beam.Application.Services;

public class UserAuthService : IUserAuthService
{
   private readonly BeamDbContext _dbContext;
   private readonly IUserActivityService _userActivityService;
   private readonly IHubContext<ChatHub> _hubContext;

   public UserAuthService(IUserActivityService userActivityService, BeamDbContext dbContext,
      IHubContext<ChatHub> hubContext)
   {
      _dbContext = dbContext;
      _userActivityService = userActivityService;
      _hubContext = hubContext;
   }
   public async Task LogoutAsync(Guid userId)
   {
      var user = await _dbContext.Users.FindAsync(userId);
      if (user != null)
      {
         user.IsOnline = false;
         await _userActivityService.UpdateLastActivityAsync(userId);
         await _dbContext.SaveChangesAsync();

         await _hubContext.Clients.All.SendAsync("UserLIstUpdated");
      }
   }
   public async Task<UserDto> ValidateUserAsync(string tag, string password)
   {
      var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Tag == tag);
      if (user == null) throw new NotFoundException("Пользователь не найден.");

      var isPasswordValid = PasswordHasher.VerifyPassword(user.PasswordHash, password);
      if (!isPasswordValid) throw new BadRequestException("Неверный пароль.");

      return new UserDto
      {
         Id = user.Id,
         Name = user.Name,
         Tag = user.Tag,
         CreationDate = user.CreationDate,
         IsOnline = user.IsOnline
      };
   }
}