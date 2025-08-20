using SafeProjectName.Models;

namespace SafeProjectName.Interfaces;

public interface IScoreService
{
	Task<Score> CreateScoreAsync(Score user);
	Task<IEnumerable<Score>> GetAllScoresAsync();
	Task<Score?> GetScoreByIdAsync(int id);
	Task<Score?> UpdateScoreAsync(int id, Score user);
	Task<bool> DeleteScoreAsync(int id);
}
