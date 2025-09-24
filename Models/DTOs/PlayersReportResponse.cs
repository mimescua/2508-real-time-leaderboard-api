namespace SafeProjectName.Models.DTOs;
public class PlayersReportResponse
{
	public required int UserId { get; set; }
	public required string Username { get; set; } = string.Empty;
	public required int TotalScore { get; set; }
	public required int MaximumScore { get; set; }
	public required int MinimumScore { get; set; }
	public required double AverageScore { get; set; }
}
