using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface ILeaderboardService
{
	Task<PaginatedList<LeaderboardResponse>> GetGlobalTopScores(int pageIndex, int pageSize);
	Task<PaginatedList<LeaderboardResponse>> GetGameTopScores(int gameId, int pageIndex, int pageSize);
	Task<PaginatedList<RankingResponse>> GetGlobalUserRanking(int userId, int pageIndex, int pageSize);
	Task<RankingResponse> GetGameUserRanking(int userId, int gameId);
}
