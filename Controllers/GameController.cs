using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SafeProjectName.Interfaces;
using SafeProjectName.Models.DTOs;

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

	[HttpGet("All", Name = "GetAllGames")]
	[Authorize]
	public async Task<IActionResult> GetAllGames()
	{
		var games = await _service.GetAllGamesAsync();
		return Ok(new { message = "Games retrieved successfully", data = games });
	}

	[HttpGet("{id}", Name = "GetGameById")]
	[Authorize]
	public async Task<IActionResult> GetGameById(int id)
	{
		var game = await _service.GetGameByIdAsync(id);
		return Ok(new { message = $"Game {id} retrieved successfully", data = game });
	}

	[HttpPost("Create", Name = "CreateGame")]
	[Authorize]
	public async Task<IActionResult> CreateGame([FromBody] GameCreateRequest game)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(game.Title))
			{
				throw new BadHttpRequestException("Invalid Title", new Exception("Title cannot be null or whitespace"));
			}

			if (game.Genres.IsNullOrEmpty())
			{
				throw new BadHttpRequestException("Invalid Genres", new Exception("Genres cannot be null or whitespace"));
			}

			if (string.IsNullOrWhiteSpace(game.ReleaseDate))
			{
				throw new BadHttpRequestException("Invalid Release Date", new Exception("Release date cannot be null or whitespace"));
			}

			await _service.CreateGameAsync(game);
			return Ok(new { message = "Game successfully created" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Invalid game creation parameters");
			throw;
		}
	}

	[HttpPut("{id}", Name = "UpdateGame")]
	[Authorize]
	public async Task<IActionResult> UpdateGame(int id, [FromBody] GameUpdateRequest game)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(game.Title)
			&& string.IsNullOrWhiteSpace(game.About)
			&& string.IsNullOrWhiteSpace(game.Platforms)
			&& game.Genres.IsNullOrEmpty()
			&& string.IsNullOrWhiteSpace(game.ReleaseDate))
			{
				throw new BadHttpRequestException("Invalid parameters",
				new Exception("Title, About, Platforms, Genres and Release Date cannot all be empty"));
			}

			await _service.UpdateGameAsync(id, game);
			return Ok(new { message = $"Game {id} successfully updated" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Invalid for game update parameters");
			throw;
		}
	}

	[HttpDelete("{id}", Name = "DeleteGame")]
	[Authorize]
	public async Task<IActionResult> DeleteGame(int id)
	{
		await _service.DeleteGameAsync(id);
		return Ok(new { message = $"Game {id} successfully deleted" });
	}
}
