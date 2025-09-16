namespace SafeProjectName.Models.DTOs;
public class ScoreRequest
{
	public required int Value { get; set; }
	public required int UserId { get; set; }
	public required int GameId { get; set; }
}
