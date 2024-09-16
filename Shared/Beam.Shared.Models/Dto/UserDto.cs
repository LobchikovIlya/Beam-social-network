namespace Beam.Shared.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public string Tag { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreationDate { get; set; }
}