using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Services;

public class LeaderboardService : ILeaderboardService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<LeaderboardService> _logger;

	public LeaderboardService(LeaderBoardDbContext dbcontext, ILogger<LeaderboardService> logger)
	{
		_dbcontext = dbcontext;
		_logger = logger;
	}

	public async Task<PaginatedList<LeaderboardResponse>> GetGlobalTopScores(int pageIndex, int pageSize)
	{
		try
		{
			int totalScores = await _dbcontext.scores.CountAsync();
			int totalPages = (int)Math.Ceiling(totalScores / (double)pageSize);

			var scores = await _dbcontext.scores
				.Include(s => s.User)
				.Include(s => s.Game)
				.OrderByDescending(s => s.Value)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var rankingResponses = scores.Select(score => new LeaderboardResponse
			{
				ScoreId = score.ScoreId,
				Value = score.Value,
				UserId = score.User!.UserId,
				Username = score.User.Username,
				GameId = score.Game!.GameId,
				Gametitle = score.Game.Title,
				Genres = GameInfoMapper.GetGenreNames(score.Game.Genres),
				AchievedAt = score.AchievedAt.ToString("yyyy-MM-dd HH:mm:ss"),
			}).ToList();

			return new PaginatedList<LeaderboardResponse>(rankingResponses, pageIndex, totalPages);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving player rankings");
			throw;
		}
	}

	public async Task<PaginatedList<LeaderboardResponse>> GetGameTopScores(int gameId, int pageIndex, int pageSize)
	{
		try
		{
			if (!await _dbcontext.games.AnyAsync(g => g.GameId == gameId))
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			int totalScores = await _dbcontext.scores.CountAsync();
			int totalPages = (int)Math.Ceiling(totalScores / (double)pageSize);

			var scores = await _dbcontext.scores
				.Include(s => s.User)
				.Include(s => s.Game)
				.Where(s => s.GameId == gameId)
				.OrderByDescending(s => s.Value)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var rankingResponses = scores.Select(score => new LeaderboardResponse
			{
				ScoreId = score.ScoreId,
				Value = score.Value,
				UserId = score.User!.UserId,
				Username = score.User.Username,
				GameId = score.Game!.GameId,
				Gametitle = score.Game.Title,
				Genres = GameInfoMapper.GetGenreNames(score.Game.Genres),
				AchievedAt = score.AchievedAt.ToString("yyyy-MM-dd HH:mm:ss"),
			}).ToList();

			return new PaginatedList<LeaderboardResponse>(rankingResponses, pageIndex, totalPages);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving player rankings");
			throw;
		}
	}

	public async Task<PaginatedList<RankingResponse>> GetGlobalUserRanking(int userId, int pageIndex, int pageSize)
	{
		try
		{
			if (!await _dbcontext.users.AnyAsync(u => u.UserId == userId))
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			int totalScores = await _dbcontext.scores.Where(score => score.UserId == userId).CountAsync();
			int totalPages = (int)Math.Ceiling(totalScores / (double)pageSize);

			var scores = await _dbcontext.scores
				.Include(s => s.Game)
				.Where(s => s.UserId == userId)
				.Select(s => new
				{
					s.ScoreId,
					s.UserId,
					s.Value,
					s.Game,
					s.AchievedAt,
					Rank = 1 + _dbcontext.scores.Count(x => x.Value > s.Value)
				})
				.OrderBy(s => s.Rank)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();


			var rankingResponses = scores.Select(score => new RankingResponse
			{
				ScoreId = score.ScoreId,
				Value = score.Value,
				Rank = score.Rank,
				GameId = score.Game!.GameId,
				Gametitle = score.Game.Title,
				Genres = GameInfoMapper.GetGenreNames(score.Game.Genres),
				AchievedAt = score.AchievedAt.ToString("yyyy-MM-dd HH:mm:ss"),
			}).ToList();

			return new PaginatedList<RankingResponse>(rankingResponses, pageIndex, totalPages);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving global player rank");
			throw;
		}
	}

	public async Task<RankingResponse> GetGameUserRanking(int userId, int gameId)
	{
		try
		{
			if (!await _dbcontext.users.AnyAsync(u => u.UserId == userId))
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			if (!await _dbcontext.games.AnyAsync(g => g.GameId == gameId))
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			var scores = await _dbcontext.scores
				.Include(s => s.Game)
				.Where(s => s.UserId == userId && s.GameId == gameId)
				.Select(s => new
				{
					s.ScoreId,
					s.UserId,
					s.Value,
					s.Game,
					s.AchievedAt,
					Rank = 1 + _dbcontext.scores.Count(x => x.Value > s.Value)
				})
				.FirstOrDefaultAsync();

			var rankingResponses = new RankingResponse
			{
				ScoreId = scores!.ScoreId,
				Value = scores.Value,
				Rank = scores.Rank,
				GameId = scores.Game!.GameId,
				Gametitle = scores.Game.Title,
				Genres = GameInfoMapper.GetGenreNames(scores.Game.Genres),
				AchievedAt = scores.AchievedAt.ToString("yyyy-MM-dd HH:mm:ss"),
			};

			return rankingResponses;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving game player rank");
			throw;
		}
	}
}
