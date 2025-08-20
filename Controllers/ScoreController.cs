using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;

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

	[HttpGet(Name = "GetAllScores")]
	[Authorize]
	public async Task<IActionResult> GetAllScores()
	{
		var result = await _service.GetAllScoresAsync();
		return Ok(result);
	}

	[HttpGet("{id}", Name = "GetScoreById")]
	public async Task<IActionResult> GetScoreById(int id)
	{
		var user = await _service.GetScoreByIdAsync(id);
		if (user == null)
		{
			return NotFound();
		}
		return Ok(user);
	}

	[HttpPost(Name = "CreateScore")]
	[Authorize]
	public async Task<IActionResult> CreateScore([FromBody] Score user)
	{
		if (user == null)
		{
			return BadRequest("Score cannot be null");
		}

		var createdScore = await _service.CreateScoreAsync(user);
		return CreatedAtAction(nameof(GetScoreById), new { id = createdScore.ScoreId }, createdScore);
	}

	[HttpPut("{id}", Name = "UpdateScore")]
	[Authorize]
	public async Task<IActionResult> UpdateScore(int id, [FromBody] Score user)
	{
		if (user == null || id != user.ScoreId)
		{
			return BadRequest("Score data is invalid");
		}

		var updatedScore = await _service.UpdateScoreAsync(id, user);
		if (updatedScore == null)
		{
			return NotFound();
		}
		return Ok(updatedScore);
	}

	[HttpDelete("{id}", Name = "DeleteScore")]
	[Authorize]
	public async Task<IActionResult> DeleteScore(int id)
	{
		bool result = await _service.DeleteScoreAsync(id);
		if (!result)
		{
			return NotFound();
		}
		return NoContent();
	}
}
