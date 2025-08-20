namespace SafeProjectName.Models.DTOs;

public class TokenDto
{
	public required string AccessToken { get; set; } = string.Empty;
	public required string RefreshToken { get; set; } = string.Empty;
}
