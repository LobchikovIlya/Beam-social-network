using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Beam.Application.Services.Interfaces;
using Beam.Shared.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Beam.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRoleService _userRoleService;

    public TokenService(IConfiguration configuration,IUserRoleService userRoleService)
    {
        _configuration = configuration;
        _userRoleService = userRoleService;
    }

    public async Task<string> GenerateTokenAsync(UserDto user)
    
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Tag),
            new Claim(ClaimTypes.Name, user.Name)    
        };
        var roles = await _userRoleService.GetRolesToUserAsync(user.Id);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}