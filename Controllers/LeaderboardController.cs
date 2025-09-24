using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeProjectName.Interfaces;

namespace SafeProjectName.Controllers;

[ApiController]
[Route("[controller]")]
public class LeaderboardController : ControllerBase
{

	private readonly ILeaderboardService _service;
	private readonly ILogger<LeaderboardController> _logger;

	public LeaderboardController(ILogger<LeaderboardController> logger, ILeaderboardService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet("Global", Name = "GetGlobalTopScores")]
	[Authorize]
	public async Task<IActionResult> GetGlobalTopScores(int pageIndex = 1, int pageSize = 10)
	{
		try
		{
			if (pageIndex <= 0 || pageSize <= 0)
			{
				throw new BadHttpRequestException("Invalid Pagination Parameters", new Exception("Page index and page size must be greater than zero"));
			}

			var scores = await _service.GetGlobalTopScores(pageIndex, pageSize);
			return Ok(new { message = $"Global leaderboard page {pageIndex} retrieved successfully", data = scores });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving scores");
			throw;
		}
	}

	[HttpGet("Game/{gameId}", Name = "GetGameTopScores")]
	[Authorize]
	public async Task<IActionResult> GetGameTopScores(int gameId, int pageIndex = 1, int pageSize = 10)
	{
		try
		{
			if (pageIndex <= 0 || pageSize <= 0)
			{
				throw new BadHttpRequestException("Invalid Pagination Parameters", new Exception("Page index and page size must be greater than zero"));
			}

			var scores = await _service.GetGameTopScores(gameId, pageIndex, pageSize);
			return Ok(new { message = $"Game leaderboard page {pageIndex} retrieved successfully", data = scores });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving game scores");
			throw;
		}
	}

	[HttpGet("Rank/{userId}", Name = "GetGlobalUserRanking")]
	[Authorize]
	public async Task<IActionResult> GetGlobalUserRanking(int userId, int pageIndex = 1, int pageSize = 10)
	{
		try
		{
			if (pageIndex <= 0 || pageSize <= 0)
			{
				throw new BadHttpRequestException("Invalid Pagination Parameters", new Exception("Page index and page size must be greater than zero"));
			}

			var scores = await _service.GetGlobalUserRanking(userId, pageIndex, pageSize);
			return Ok(new { message = $"User rank page {pageIndex} retrieved successfully", data = scores });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving user ranking");
			throw;
		}
	}

	[HttpGet("Game/{gameId}/Rank/{userId}", Name = "GetGameUserRanking")]
	[Authorize]
	public async Task<IActionResult> GetGameUserRanking(int userId, int gameId)
	{
		try
		{
			var score = await _service.GetGameUserRanking(userId, gameId);
			return Ok(new { message = $"User rank in game {gameId} retrieved successfully", data = score });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving user ranking");
			throw;
		}
	}
}
