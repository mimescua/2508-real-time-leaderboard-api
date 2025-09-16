namespace SafeProjectName.Models.DTOs;
public class ScoreResponse
{
	public required int ScoreId { get; set; }
	public required int Value { get; set; }
	public required string AchievedAt { get; set; } = string.Empty;
	public required int UserId { get; set; }
	public required int GameId { get; set; }
}
