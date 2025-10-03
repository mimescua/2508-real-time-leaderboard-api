using Microsoft.EntityFrameworkCore;
using SafeProjectName.Constants;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;

namespace SafeProjectName.Services;

public class ScoreService : IScoreService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<ScoreService> _logger;
	private ICacheService _cacheservice;

	public ScoreService(LeaderBoardDbContext dbcontext, ILogger<ScoreService> logger, ICacheService cacheService)
	{
		_dbcontext = dbcontext;
		_logger = logger;
		_cacheservice = cacheService;
	}

	public async Task SubmitScoreAsync(int gameId, int userId, int value)
	{
		try
		{
			var user = await _dbcontext.users.FindAsync(userId);
			if (user == null)
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			var game = await _dbcontext.games.FindAsync(gameId);
			if (game == null)
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			var existingScore = await _dbcontext.scores.FirstOrDefaultAsync(score => score.GameId == gameId && score.UserId == userId);
			int gameScore = 0;
			if (existingScore != null)
			{
				existingScore.Value += value;
				existingScore.AchievedAt = DateTime.UtcNow;

				gameScore = existingScore.Value;
			}
			else
			{
				var score = new Score
				{
					Value = value,
					AchievedAt = DateTime.UtcNow,
					UserId = userId,
					GameId = gameId,
				};

				gameScore = score.Value;
				_dbcontext.Add(score);
			}
			await _dbcontext.SaveChangesAsync();

			await _cacheservice.AddOrUpdateSortedSetAsync($"{ScoreKeys.GamePrefix}{gameId}", $"{user.Username}", gameScore);
			await _cacheservice.AddOrUpdateSortedSetAsync($"{ScoreKeys.UserPrefix}{userId}", $"{game.Title}", gameScore);

			double currentGlobalScore = await _cacheservice.GetSingleSortedSetScoreAsync($"{ScoreKeys.GlobalPrefix}", $"{user.Username}");
			double updatedGlobalScore = currentGlobalScore + gameScore;
			await _cacheservice.AddOrUpdateSortedSetAsync($"{ScoreKeys.GlobalPrefix}", $"{user.Username}", updatedGlobalScore);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while handling the score");
			throw;
		}
	}
}
