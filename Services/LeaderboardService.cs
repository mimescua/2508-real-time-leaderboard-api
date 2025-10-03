using Microsoft.EntityFrameworkCore;
using SafeProjectName.Constants;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Services;

public class LeaderboardService : ILeaderboardService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<LeaderboardService> _logger;
	private ICacheService _cacheservice;

	public LeaderboardService(LeaderBoardDbContext dbcontext, ILogger<LeaderboardService> logger, ICacheService cacheService)
	{
		_dbcontext = dbcontext;
		_logger = logger;
		_cacheservice = cacheService;
	}

	public async Task<PaginatedList<LeaderboardEntry>> GetGlobalTopScores(int pageIndex, int pageSize)
	{
		try
		{
			long sortedSetLength = await _cacheservice.CountSortedSetAsync($"{ScoreKeys.GlobalPrefix}");
			int totalPages = sortedSetLength == 0 ? 0 : (int)Math.Ceiling(sortedSetLength / (double)pageSize);
			if (sortedSetLength > 0)
			{
				var entries = await _cacheservice.GetSortedSetRangeAsync($"{ScoreKeys.GlobalPrefix}", pageIndex, pageSize);
				var results = new List<LeaderboardEntry>();
				for (int i = 0; i < entries.Length; i++)
				{
					results.Add(new LeaderboardEntry
					(
						member: entries[i].Element.ToString(),
						score: entries[i].Score,
						rank: i + 1
					));
				}
				return new PaginatedList<LeaderboardEntry>(results, pageIndex, totalPages);
			}
			int totalScores = await _dbcontext.scores.CountAsync();
			totalPages = (int)Math.Ceiling(totalScores / (double)pageSize);

			var scores = await _dbcontext.scores
				.Include(s => s.User)
				.OrderByDescending(s => s.Value)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var rankingResponses = scores.Select((score, index) => new LeaderboardEntry(
				member: score.User!.Username,
				score: score.Value,
				rank: index + 1 + (pageIndex - 1) * pageSize
			)).ToList();

			return new PaginatedList<LeaderboardEntry>(rankingResponses, pageIndex, totalPages);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving player rankings");
			throw;
		}
	}

	public async Task<PaginatedList<LeaderboardEntry>> GetGameTopScores(int gameId, int pageIndex, int pageSize) // top x game
	{
		try
		{
			if (!await _dbcontext.games.AnyAsync(g => g.GameId == gameId))
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			long sortedSetLength = await _cacheservice.CountSortedSetAsync($"{ScoreKeys.GamePrefix}{gameId}");
			int totalPages = sortedSetLength == 0 ? 0 : (int)Math.Ceiling(sortedSetLength / (double)pageSize);
			if (sortedSetLength > 0)
			{
				var entries = await _cacheservice.GetSortedSetRangeAsync($"{ScoreKeys.GamePrefix}{gameId}", pageIndex, pageSize);
				var results = new List<LeaderboardEntry>();
				for (int i = 0; i < entries.Length; i++)
				{
					results.Add(new LeaderboardEntry
					(
						member: entries[i].Element.ToString(),
						score: entries[i].Score,
						rank: i + 1
					));
				}
				return new PaginatedList<LeaderboardEntry>(results, pageIndex, totalPages);
			}
			int totalScores = await _dbcontext.scores.CountAsync();
			totalPages = (int)Math.Ceiling(totalScores / (double)pageSize);

			var scores = await _dbcontext.scores
				.Include(s => s.User)
				.Where(s => s.GameId == gameId)
				.OrderByDescending(s => s.Value)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var rankingResponses = scores.Select((score, index) => new LeaderboardEntry(
				member: score.User!.Username,
				score: score.Value,
				rank: index + 1 + (pageIndex - 1) * pageSize
			)).ToList();

			return new PaginatedList<LeaderboardEntry>(rankingResponses, pageIndex, totalPages);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving player rankings");
			throw;
		}
	}

	public async Task<PaginatedList<LeaderboardEntry>> GetGlobalUserRanking(int userId, int pageIndex, int pageSize) // scores and position by user
	{
		try
		{
			if (!await _dbcontext.users.AnyAsync(u => u.UserId == userId))
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			long sortedSetLength = await _cacheservice.CountSortedSetAsync($"{ScoreKeys.UserPrefix}{userId}");
			int totalPages = sortedSetLength == 0 ? 0 : (int)Math.Ceiling(sortedSetLength / (double)pageSize);
			if (sortedSetLength > 0)
			{
				var entries = await _cacheservice.GetSortedSetRangeAsync($"{ScoreKeys.UserPrefix}{userId}", pageIndex, pageSize);
				var results = new List<LeaderboardEntry>();
				for (int i = 0; i < entries.Length; i++)
				{
					results.Add(new LeaderboardEntry
					(
						member: entries[i].Element.ToString(),
						score: entries[i].Score,
						rank: i + 1
					));
				}
				return new PaginatedList<LeaderboardEntry>(results, pageIndex, totalPages);
			}
			int totalScores = await _dbcontext.scores.CountAsync();
			totalPages = (int)Math.Ceiling(totalScores / (double)pageSize);

			var scores = await _dbcontext.scores
				.Include(s => s.User)
				.Where(s => s.UserId == userId)
				.Select(s => new
				{
					s.Value,
					s.User,
					Rank = 1 + _dbcontext.scores.Count(x => x.Value > s.Value)
				})
				.OrderBy(s => s.Rank)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var rankingResponses = scores.Select((score, index) => new LeaderboardEntry(
				member: score.User!.Username,
				score: score.Value,
				rank: score.Rank
			)).ToList();

			return new PaginatedList<LeaderboardEntry>(rankingResponses, pageIndex, totalPages);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving global player rank");
			throw;
		}
	}

	public async Task<LeaderboardEntry> GetGameUserRanking(int userId, int gameId) // single score and position by user and game
	{
		try
		{
			var user = await _dbcontext.users.FindAsync(userId);
			if (user == null)
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			if (!await _dbcontext.games.AnyAsync(g => g.GameId == gameId))
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			var (score, rank) = await _cacheservice.GetSingleSortedSetAsync($"{ScoreKeys.GamePrefix}{gameId}", $"{user.Username}");
			if (score != null && rank != null)
			{
				return new LeaderboardEntry
				(
					member: user.Username,
					score: score.Value,
					rank: 1 + rank.Value
				);
			}

			var result = await _dbcontext.scores
				.Include(s => s.User)
				.Where(s => s.UserId == userId && s.GameId == gameId)
				.OrderByDescending(s => s.Value)
				.Select(s => new
				{
					s.Value,
					s.User,
					Rank = 1 + _dbcontext.scores.Count(x => x.Value > s.Value && x.GameId == gameId)
				})
				.FirstOrDefaultAsync();

			if (result == null)
			{
				throw new KeyNotFoundException("No score found for the given user and game");
			}
			return new LeaderboardEntry
			(
				member: result.User!.Username,
				score: result.Value,
				rank: result.Rank
			);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving game player rank");
			throw;
		}
	}
}
