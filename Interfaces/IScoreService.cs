namespace SafeProjectName.Interfaces;

public interface IScoreService
{
	Task SubmitScoreAsync(int gameId, int userId, int value);
}
