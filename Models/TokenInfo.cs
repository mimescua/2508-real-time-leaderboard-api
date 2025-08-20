namespace SafeProjectName.Models;

public class TokenInfo
{
	public int TokenId { get; set; }
	public string Username { get; set; } = string.Empty;
	public string RefreshToken { get; set; } = string.Empty;
	public DateTime ExpiredAt { get; set; }
}
