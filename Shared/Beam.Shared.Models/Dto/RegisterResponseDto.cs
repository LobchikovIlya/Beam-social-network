namespace Beam.Shared.Dto;

public class RegisterResponseDto
{
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
}