namespace SafeProjectName.Models.DTOs;

public class UserUpdateDto
{
	public required string Username { get; set; } = string.Empty;
	public required string Email { get; set; } = string.Empty;
}
