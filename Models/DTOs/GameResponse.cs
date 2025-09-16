namespace SafeProjectName.Models.DTOs;

public class GameResponse
{
	public required int GameId { get; set; }
	public required string Title { get; set; } = string.Empty;
	public string? About { get; set; } = string.Empty;
	public string? Platforms { get; set; } = string.Empty;
	public required string[] Genres { get; set; } = Array.Empty<string>();
	public required string ReleaseDate { get; set; } = string.Empty;
	public ICollection<ScoreResponse>? Scores { get; set; } = Array.Empty<ScoreResponse>();
}
