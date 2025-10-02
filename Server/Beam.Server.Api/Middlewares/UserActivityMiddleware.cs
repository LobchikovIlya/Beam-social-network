using System.Security.Claims;
using Beam.Application.Services.Interfaces;

namespace Beam.Api.Middlewares;

public class UserActivityMiddleware
{
    private readonly RequestDelegate _next;
   
    public UserActivityMiddleware(RequestDelegate next)
    {
        _next = next;
      
    }

    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            await _next(context);
            return;
        }

        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            var userActivityService = context.RequestServices.GetRequiredService<IUserActivityService>();
            await userActivityService.UpdateLastActivityAsync(userId);
        }
            

        await _next(context);
    }
}