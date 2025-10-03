namespace SafeProjectName.Models.DTOs;

public class LeaderboardEntry
{
	private string Member { get; set; }
	public string Username => Member;
	public double Score { get; set; }
	public long Rank { get; set; }
	public LeaderboardEntry(string member, double score, long rank)
	{
		Member = member;
		Score = score;
		Rank = rank;
	}
}
