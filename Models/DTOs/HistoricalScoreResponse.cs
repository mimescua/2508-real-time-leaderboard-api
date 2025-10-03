namespace SafeProjectName.Models.DTOs;
public class HistoricalScoreResponse
{
	public required int ScoreId { get; set; }
	public required int Value { get; set; }
	public required int UserId { get; set; }
	public required string Username { get; set; } = string.Empty;
	public required int GameId { get; set; }
	public required string Gametitle { get; set; } = string.Empty;
	public required string[] Genres { get; set; } = Array.Empty<string>();
	public required string AchievedAt { get; set; } = string.Empty;
}
