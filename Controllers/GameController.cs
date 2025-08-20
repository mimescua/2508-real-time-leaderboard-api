using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeProjectName.DataAccess;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;

namespace SafeProjectName.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{

	private readonly IGameService _service;
	private readonly ILogger<GameController> _logger;

	public GameController(ILogger<GameController> logger, IGameService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet(Name = "GetAllGames")]
	[Authorize]
	public async Task<IActionResult> GetAllGames()
	{
		var result = await _service.GetAllGamesAsync();
		return Ok(result);
	}

	[HttpGet("{id}", Name = "GetGameById")]
	[Authorize]
	public async Task<IActionResult> GetGameById(int id)
	{
		var user = await _service.GetGameByIdAsync(id);
		if (user == null)
		{
			return NotFound();
		}
		return Ok(user);
	}

	[HttpPost(Name = "CreateGame")]
	[Authorize]
	public async Task<IActionResult> CreateGame([FromBody] Game user)
	{
		if (user == null)
		{
			return BadRequest("Game cannot be null");
		}

		var createdGame = await _service.CreateGameAsync(user);
		return CreatedAtAction(nameof(GetGameById), new { id = createdGame.GameId }, createdGame);
	}

	[HttpPut("{id}", Name = "UpdateGame")]
	[Authorize]
	public async Task<IActionResult> UpdateGame(int id, [FromBody] Game user)
	{
		if (user == null || id != user.GameId)
		{
			return BadRequest("Game data is invalid");
		}

		var updatedGame = await _service.UpdateGameAsync(id, user);
		if (updatedGame == null)
		{
			return NotFound();
		}
		return Ok(updatedGame);
	}

	[HttpDelete("{id}", Name = "DeleteGame")]
	[Authorize]
	public async Task<IActionResult> DeleteGame(int id)
	{
		bool result = await _service.DeleteGameAsync(id);
		if (!result)
		{
			return NotFound();
		}
		return NoContent();
	}
}
