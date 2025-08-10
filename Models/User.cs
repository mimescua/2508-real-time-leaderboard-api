namespace SafeProjectName.Models;
public class User
{
	public int UserId { get; set; }
	public required string Username { get; set; }
	public required string Email { get; set; }
	public required string Password { get; set; }
	public DateTime CreatedAt { get; set; }

	public virtual ICollection<Score>? Scores { get; set; }
}
