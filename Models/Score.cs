namespace SafeProjectName.Models;
public class Score
{
	public int ScoreId { get; set; }
	public int Value { get; set; }

	public DateTime AchievedAt { get; set; }

	public int UserId { get; set; }
	public virtual User? User { get; set; }

	public int GameId { get; set; }
	public virtual Game? Game { get; set; }
}
