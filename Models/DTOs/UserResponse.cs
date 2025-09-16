namespace SafeProjectName.Models.DTOs;

public class UserResponse
{
	public required int UserId { get; set; }
	public required string Username { get; set; } = string.Empty;
	public required string Email { get; set; } = string.Empty;
	public required string CreatedAt { get; set; } = string.Empty;
	public ScoreResponse[]? Scores { get; set; } = Array.Empty<ScoreResponse>();
}
