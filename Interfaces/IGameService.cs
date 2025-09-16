using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface IGameService
{
	Task CreateGameAsync(GameCreateRequest request);
	Task<IEnumerable<GameResponse>> GetAllGamesAsync();
	Task<GameResponse> GetGameByIdAsync(int id);
	Task UpdateGameAsync(int id, GameUpdateRequest request);
	Task DeleteGameAsync(int id);
}
