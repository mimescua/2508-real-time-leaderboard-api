using SafeProjectName.Models.Enums;

namespace SafeProjectName.Models;

public class Game
{
	public int GameId { get; set; }
	public required string Title { get; set; }
	public string? About { get; set; }
	public string? Platforms { get; set; }
	public GameGenre Genres { get; set; }
	public DateTime ReleaseDate { get; set; }
	public virtual ICollection<Score>? Scores { get; set; }
}
