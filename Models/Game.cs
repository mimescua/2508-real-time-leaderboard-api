namespace SafeProjectName.Models;

public enum GameGenre : ushort
{
	None = 0,
	Action = 1 << 0,
	Adventure = 1 << 1,
	ActionAventure = Action | Adventure,
	Educational = 1 << 2,
	Fighting = 1 << 3,
	Party = 1 << 4,
	Platform = 1 << 5,
	Puzzle = 1 << 6,
	Racing = 1 << 7,
	RolePlaying = 1 << 8,
	Sandbox = 1 << 9,
	Shooter = 1 << 10,
	Simulation = 1 << 11,
	Sports = 1 << 12,
	Strategy = 1 << 13
}

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
