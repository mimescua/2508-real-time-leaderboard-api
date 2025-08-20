using SafeProjectName.Models;

namespace SafeProjectName.Interfaces;

public interface IGameService
{
	Task<Game> CreateGameAsync(Game user);
	Task<IEnumerable<Game>> GetAllGamesAsync();
	Task<Game?> GetGameByIdAsync(int id);
	Task<Game?> UpdateGameAsync(int id, Game user);
	Task<bool> DeleteGameAsync(int id);
}
