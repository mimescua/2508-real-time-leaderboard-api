using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;

namespace SafeProjectName.Services;
public class ScoreService : IScoreService
{
	private readonly LeaderBoardDbContext _dbcontext;

	public ScoreService(LeaderBoardDbContext dbcontext)
	{
		_dbcontext = dbcontext;
	}

	public async Task<Score> CreateScoreAsync(Score score)
	{
		_dbcontext.Add(score);
		await _dbcontext.SaveChangesAsync();
		return score;
	}
	public async Task<IEnumerable<Score>> GetAllScoresAsync()
	{
		return await _dbcontext.scores.ToListAsync();
	}
	public async Task<Score?> GetScoreByIdAsync(int id)
	{
		return await _dbcontext.scores.FindAsync(id) ?? null;
	}

	public async Task<Score?> UpdateScoreAsync(int id, Score updatedScore)
	{
		var existingScore = await _dbcontext.scores.FindAsync(id);

		if (existingScore == null) { return null; }

		existingScore.Value = updatedScore.Value;
		existingScore.AchievedAt = updatedScore.AchievedAt;
		existingScore.UserId = updatedScore.UserId;
		existingScore.GameId = updatedScore.GameId;

		await _dbcontext.SaveChangesAsync();
		return existingScore;
	}
	public async Task<bool> DeleteScoreAsync(int id)
	{
		var existingScore = _dbcontext.scores.Find(id);

		if (existingScore == null) { return false; }

		_dbcontext.Remove(existingScore);
		await _dbcontext.SaveChangesAsync();
		return true;
	}
}
