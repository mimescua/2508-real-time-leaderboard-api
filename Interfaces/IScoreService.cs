using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface IScoreService
{
	Task SubmitScoreAsync(ScoreRequest request);
}
