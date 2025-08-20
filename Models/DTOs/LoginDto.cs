namespace SafeProjectName.Models.DTOs;

public class LoginDto
{
	public required string Username { get; set; } = string.Empty;
	public required string Password { get; set; } = string.Empty;
}
