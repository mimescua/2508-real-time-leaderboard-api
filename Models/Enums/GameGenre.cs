namespace SafeProjectName.Models.Enums;

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
