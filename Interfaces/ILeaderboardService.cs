using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface ILeaderboardService
{
	Task<PaginatedList<LeaderboardEntry>> GetGlobalTopScores(int pageIndex, int pageSize);
	Task<PaginatedList<LeaderboardEntry>> GetGameTopScores(int gameId, int pageIndex, int pageSize);
	Task<PaginatedList<LeaderboardEntry>> GetGlobalUserRanking(int userId, int pageIndex, int pageSize);
	Task<LeaderboardEntry> GetGameUserRanking(int userId, int gameId);
}
