using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Services;
public class ScoreService : IScoreService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<GameService> _logger;

	public ScoreService(LeaderBoardDbContext dbcontext, ILogger<GameService> logger)
	{
		_dbcontext = dbcontext;
		_logger = logger;
	}

	public async Task SubmitScoreAsync(ScoreRequest request)
	{
		try
		{
			var existingGame = await _dbcontext.games.FindAsync(request.GameId);
			if (existingGame == null)
			{
				throw new KeyNotFoundException("No game found with the given ID");
			}

			var existingUser = await _dbcontext.users.FindAsync(request.UserId);
			if (existingUser == null)
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			var existingScore = await _dbcontext.scores.FirstOrDefaultAsync(score => score.GameId == request.GameId && score.UserId == request.UserId);
			if (existingScore != null)
			{
				existingScore.Value += request.Value;
				existingScore.AchievedAt = DateTime.UtcNow;
			}
			else
			{
				var score = new Score
				{
					Value = request.Value,
					AchievedAt = DateTime.UtcNow,
					UserId = request.UserId,
					GameId = request.GameId,
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
