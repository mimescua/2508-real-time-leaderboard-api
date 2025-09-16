using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;
using SafeProjectName.Models.DTOs;
using SafeProjectName.Models.Enums;

namespace SafeProjectName.Services;
public class GameService : IGameService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<GameService> _logger;

	public GameService(LeaderBoardDbContext dbcontext, ILogger<GameService> logger)
	{
		_dbcontext = dbcontext;
		_logger = logger;
	}

	public async Task CreateGameAsync(GameCreateRequest request)
	{
		try
		{
			GameGenre _genres = request.Genres
			.Select(Enum.Parse<GameGenre>)
			.Aggregate(GameGenre.None, (acc, val) => acc | val);

			bool _parsed = DateTime.TryParse(request.ReleaseDate, out DateTime _releaseDateTime);
			if (!_parsed)
			{
				throw new ArgumentException("Invalid Release Date", new Exception("Unable to parse release date, incorrect date format"));
			}

			DateOnly _today = DateOnly.FromDateTime(DateTime.Today);
			DateOnly _releaseDate = DateOnly.FromDateTime(_releaseDateTime);
			if (_releaseDate > _today)
			{
				throw new ArgumentException("Invalid Release Date", new Exception("Release date cannot be later than today"));
			}

			var game = new Game
			{
				Title = request.Title,
				About = request.About,
				Platforms = request.Platforms,
				Genres = _genres,
				ReleaseDate = _releaseDateTime
			};

			_dbcontext.Add(game);
			await _dbcontext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while creating a game");
			throw;
		}
	}

	public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
	{
		try
		{
			var games = await _dbcontext.games.ToListAsync();
			if (games == null || games.Count == 0)
			{
				throw new InvalidDataException("No game items found");
			}

			var response = games.Select(g => new GameResponse
			{
				GameId = g.GameId,
				Title = g.Title,
				About = g.About,
				Platforms = g.Platforms,
				Genres = GameInfoMapper.GetGenreNames(g.Genres),
				ReleaseDate = g.ReleaseDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
			}).ToList();

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving all games");
			throw;
		}
	}

	public async Task<GameResponse> GetGameByIdAsync(int id)
	{
		try
		{
			var game = await _dbcontext.games
			.Where(u => u.GameId == id)
			.Select(u => new GameResponse
			{
				GameId = u.GameId,
				Title = u.Title,
				About = u.About,
				Platforms = u.Platforms,
				Genres = GameInfoMapper.GetGenreNames(u.Genres),
				ReleaseDate = u.ReleaseDate.ToString("yyyy-MM-dd HH:mm:ss"),
				Scores = u.Scores != null
					? u.Scores.Select(s => new ScoreResponse
					{
						ScoreId = s.ScoreId,
						Value = s.Value,
						AchievedAt = s.AchievedAt.ToString("yyyy-MM-dd HH:mm:ss"),
						UserId = s.UserId,
						GameId = s.GameId
					}).ToArray()
					: Array.Empty<ScoreResponse>()
			})
			.FirstOrDefaultAsync();

			if (game == null)
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			return game;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving a game by ID");
			throw;
		}
	}

	public async Task UpdateGameAsync(int id, GameUpdateRequest request)
	{
		try
		{
			var existingGame = await _dbcontext.games.FindAsync(id);
			if (existingGame == null)
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			var genres = request.Genres?
				.Select(Enum.Parse<GameGenre>)
				.Aggregate(GameGenre.None, (acc, val) => acc | val);

			DateTime releaseDate = DateTime.TryParse(request.ReleaseDate, out var parsed)
				? parsed
				: existingGame.ReleaseDate;

			DateOnly _spacewarReleaseDate = DateOnly.FromDateTime(new DateTime(1962, 02, 14));
			DateOnly _parsedDate = DateOnly.FromDateTime(parsed);
			if (_parsedDate < _spacewarReleaseDate)
			{
				throw new ArgumentException("Invalid Release Date", new Exception("Unable to parse release date, incorrect date format"));
			}

			existingGame.Title = request.Title ?? existingGame.Title;
			existingGame.About = !string.IsNullOrWhiteSpace(request.About) ? request.About : existingGame.About;
			existingGame.Platforms = !string.IsNullOrWhiteSpace(request.Platforms) ? request.Platforms : existingGame.Platforms;
			existingGame.Genres = genres ?? existingGame.Genres;
			existingGame.ReleaseDate = releaseDate;

			await _dbcontext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while updating a game");
			throw;
		}
	}

	public async Task DeleteGameAsync(int id)
	{
		try
		{
			var existingGame = _dbcontext.games.Find(id);
			if (existingGame == null)
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			_dbcontext.Remove(existingGame);
			await _dbcontext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while deleting a game");
			throw;
		}
	}
}
