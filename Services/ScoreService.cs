using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;

namespace SafeProjectName.Services;

public class ScoreService : IScoreService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<ScoreService> _logger;

	public ScoreService(LeaderBoardDbContext dbcontext, ILogger<ScoreService> logger)
	{
		_dbcontext = dbcontext;
		_logger = logger;
	}

	public async Task SubmitScoreAsync(int gameId, int userId, int value)
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

			var existingScore = await _dbcontext.scores.FirstOrDefaultAsync(score => score.GameId == gameId && score.UserId == userId);
			if (existingScore != null)
			{
				existingScore.Value += value;
				existingScore.AchievedAt = DateTime.UtcNow;
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

				_dbcontext.Add(score);
			}
			await _dbcontext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while handling the score");
			throw;
		}
	}
}
