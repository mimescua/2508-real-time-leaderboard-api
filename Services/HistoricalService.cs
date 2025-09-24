using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Services;

public class HistoricalService : IHistoricalService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<HistoricalService> _logger;

	public HistoricalService(LeaderBoardDbContext dbcontext, ILogger<HistoricalService> logger)
	{
		_dbcontext = dbcontext;
		_logger = logger;
	}

	public async Task<List<PlayersReportResponse>> GetTopPlayersReport(int gameId, DateTime fromDate, DateTime toDate, int limit = 10)
	{
		try
		{
			if (!await _dbcontext.games.AnyAsync(g => g.GameId == gameId))
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			var scores = await _dbcontext.scores
			.TemporalBetween(fromDate, toDate)
			// .Where(s => s.GameId == gameId && s.AchievedAt >= fromDate && s.AchievedAt <= toDate)
			.Where(s => s.GameId == gameId)
			.GroupBy(s => s.UserId)
			.Select(g => new
			{
				UserId = g.Key,
				TotalScore = g.Sum(x => x.Value),
				MaximumScore = g.Max(x => x.Value),
				MinimumScore = g.Min(x => x.Value),
				AverageScore = g.Average(x => x.Value)
			})
			.OrderByDescending(x => x.TotalScore)
			.Take(limit)
			.ToListAsync();

			var result = scores.Join(_dbcontext.users, s => s.UserId, u => u.UserId, (s, u) => new PlayersReportResponse
			{
				UserId = u.UserId,
				Username = u.Username,
				TotalScore = s.TotalScore,
				MaximumScore = s.MaximumScore,
				MinimumScore = s.MinimumScore,
				AverageScore = s.AverageScore
			}).ToList();

			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving top players report");
			throw;
		}
	}

	public async Task<PaginatedList<LeaderboardResponse>> GetHistoricalScores(DateTime from, DateTime to, int pageIndex, int pageSize)
	{
		try
		{
			int totalScores = await _dbcontext.scores.CountAsync();
			int totalPages = (int)Math.Ceiling(totalScores / (double)pageSize);

			var scores = await _dbcontext.scores
			.TemporalBetween(from, to)
			.Select(s => new
			{
				Score = s,
				User = _dbcontext.users.FirstOrDefault(u => u.UserId == s.UserId),
				Game = _dbcontext.games.FirstOrDefault(g => g.GameId == s.GameId)
			})
			.OrderByDescending(s => s.Score.Value)
			.Skip((pageIndex - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

			var rankingResponses = scores.Select(score =>
			{
				var (_score, _user, _game) = (score.Score, score.User, score.Game);
				return new LeaderboardResponse
				{
					ScoreId = _score.ScoreId,
					Value = _score.Value,
					UserId = _user!.UserId,
					Username = _user.Username,
					GameId = _game!.GameId,
					Gametitle = _game.Title,
					Genres = GameInfoMapper.GetGenreNames(_game.Genres),
					AchievedAt = _score.AchievedAt.ToString("yyyy-MM-dd HH:mm:ss"),
				};
			}).ToList();

			return new PaginatedList<LeaderboardResponse>(rankingResponses, pageIndex, totalPages);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving top players report");
			throw;
		}
	}
}
