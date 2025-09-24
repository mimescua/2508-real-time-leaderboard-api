using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeProjectName.Interfaces;

namespace SafeProjectName.Controllers;

[ApiController]
[Route("[controller]")]
public class HistoricalController : ControllerBase
{

	private readonly IHistoricalService _service;
	private readonly ILogger<HistoricalController> _logger;

	public HistoricalController(ILogger<HistoricalController> logger, IHistoricalService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet("TopPlayers/Global", Name = "GetHistoricallScores")]
	[Authorize]
	public async Task<IActionResult> GetHistoricalScores(DateTime fromDate, DateTime toDate, int pageIndex = 1, int pageSize = 10)
	{
		try
		{
			if (pageIndex <= 0 || pageSize <= 0)
			{
				throw new BadHttpRequestException("Invalid Pagination Parameters", new Exception("Page index and page size must be greater than zero"));
			}

			if (fromDate >= toDate)
			{
				throw new BadHttpRequestException("Invalid Date Range", new Exception("From date must be earlier than To date"));
			}

			var scores = await _service.GetHistoricalScores(fromDate, toDate, pageIndex, pageSize);
			return Ok(new { message = $"Game {pageIndex} retrieved successfully", data = scores });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving global top players report");
			throw;
		}
	}

	[HttpGet("TopPlayers/Game/{gameId}", Name = "GetTopPlayersReport")]
	[Authorize]
	public async Task<IActionResult> GetTopPlayersReport(int gameId, DateTime fromDate, DateTime toDate, int limit = 10)
	{
		try
		{
			if (limit <= 0)
			{
				throw new BadHttpRequestException("Invalid Limit Parameter", new Exception("Limit must be greater than zero"));
			}

			if (fromDate >= toDate)
			{
				throw new BadHttpRequestException("Invalid Date Range", new Exception("From date must be earlier than To date"));
			}

			object scores = await _service.GetTopPlayersReport(gameId, fromDate, toDate, limit);
			return Ok(new { message = $"Game {gameId} retrieved successfully", data = scores });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving game top players report");
			throw;
		}
	}
}
