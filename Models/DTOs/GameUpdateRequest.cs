namespace SafeProjectName.Models.DTOs;

public class GameUpdateRequest
{
	public string? Title { get; set; } = string.Empty;
	public string? About { get; set; } = string.Empty;
	public string? Platforms { get; set; } = string.Empty;
	public string[]? Genres { get; set; } = Array.Empty<string>();
	public string? ReleaseDate { get; set; } = string.Empty;
}
