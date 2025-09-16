using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeProjectName.Interfaces;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Controllers;

[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{

	private readonly IScoreService _service;
	private readonly ILogger<ScoreController> _logger;

	public ScoreController(ILogger<ScoreController> logger, IScoreService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpPut(Name = "SubmitScore")]
	[Authorize]
	public async Task<IActionResult> SubmitScore([FromBody] ScoreRequest score)
	{

		try
		{
			if (score.Value <= 0)
			{
				throw new BadHttpRequestException("Invalid Score Value", new Exception("Score value must be greater than zero"));
			}

			await _service.SubmitScoreAsync(score);
			return Ok(new { message = $"Score successfully submited" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Invalid score submission parameters");
			throw;
		}
	}
}
