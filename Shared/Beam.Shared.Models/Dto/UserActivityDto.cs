namespace Beam.Shared.Dto;

public class UserActivityDto
{
    public Guid UserId { get; set; }
    public DateTimeOffset LastActivityTime { get; set; }
}