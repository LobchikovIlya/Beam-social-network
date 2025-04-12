using System.IdentityModel.Tokens.Jwt;
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

        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
           
            await _next(context);
            return;
        }
        
        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            await userService.UpdateLastActivityAsync(userId);
        }

        await _next(context);
    

       
    }
}