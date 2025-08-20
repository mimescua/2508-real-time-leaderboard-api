using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;

namespace SafeProjectName.Services;
public class GameService : IGameService
{
	private readonly LeaderBoardDbContext _dbcontext;

	public GameService(LeaderBoardDbContext dbcontext)
	{
		_dbcontext = dbcontext;
	}

	public async Task<Game> CreateGameAsync(Game game)
	{
		_dbcontext.Add(game);
		await _dbcontext.SaveChangesAsync();
		return game;
	}
	public async Task<IEnumerable<Game>> GetAllGamesAsync()
	{
		return await _dbcontext.games.ToListAsync();
	}
	public async Task<Game?> GetGameByIdAsync(int id)
	{
		return await _dbcontext.games.FindAsync(id) ?? null;
	}

	public async Task<Game?> UpdateGameAsync(int id, Game updatedGame)
	{
		var existingGame = await _dbcontext.games.FindAsync(id);

		if (existingGame == null) { return null; }

		existingGame.Title = updatedGame.Title;
		existingGame.About = updatedGame.About;
		existingGame.Platforms = updatedGame.Platforms;
		existingGame.Genres = updatedGame.Genres;
		existingGame.ReleaseDate = updatedGame.ReleaseDate;

		await _dbcontext.SaveChangesAsync();
		return existingGame;
	}

	public async Task<bool> DeleteGameAsync(int id)
	{
		var existingGame = _dbcontext.games.Find(id);

		if (existingGame == null) { return false; }

		_dbcontext.Remove(existingGame);
		await _dbcontext.SaveChangesAsync();
		return true;
	}
}
