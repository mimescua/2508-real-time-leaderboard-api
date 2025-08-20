using Microsoft.AspNetCore.Mvc;
using SafeProjectName.Interfaces;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAccountService _accountService;

	private readonly ILogger<AuthController> _logger;

	public AuthController(ILogger<AuthController> logger, IAccountService accountService)
	{
		_logger = logger;
		_accountService = accountService;
	}

	[HttpPost("Login")]
	public async Task<IActionResult> Login([FromBody] LoginDto model)
	{
		if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
		{
			return BadRequest("Invalid parameters");
		}

		var result = await _accountService.LoginAsync(model.Username, model.Password);

		if (!result.IsSuccess)
		{
			return BadRequest(result.ErrorMessage);
		}

		return Ok(result.token);
	}

	[HttpPost("Logout")]
	public async Task<IActionResult> Logout(string AccessToken)
	{
		bool result = await _accountService.LogoutAsync(AccessToken);

		if (!result)
		{
			return NoContent();
		}

		return Ok("User logged out successfully");
	}

	[HttpPost("RefreshToken")]
	public async Task<IActionResult> RefreshToken([FromBody] TokenDto model)
	{
		if (model == null || string.IsNullOrEmpty(model.AccessToken) || string.IsNullOrEmpty(model.RefreshToken))
		{
			return BadRequest("Invalid parameters");
		}

		var result = await _accountService.RefreshTokenAsync(model);

		if (!result.IsSuccess)
		{
			return BadRequest(result.ErrorMessage);
		}

		return Ok(result.token);
	}
}
