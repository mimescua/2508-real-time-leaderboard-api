using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface IHistoricalService
{
	Task<List<PlayersReportResponse>> GetTopPlayersReport(int gameId, DateTime fromDate, DateTime toDate, int limit = 10);
	Task<PaginatedList<HistoricalScoreResponse>> GetHistoricalScores(DateTime from, DateTime to, int pageIndex, int pageSize);
}
