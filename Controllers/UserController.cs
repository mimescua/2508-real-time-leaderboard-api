using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
	private readonly IUserService _service;
	private readonly ILogger<UserController> _logger;

	public UserController(ILogger<UserController> logger, IUserService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet(Name = "GetAllUsers")]
	[Authorize]
	public async Task<IActionResult> GetAllUsers()
	{
		var result = await _service.GetAllUsersAsync();
		return Ok(result);
	}

	[HttpGet("{id}", Name = "GetUserById")]
	[Authorize]
	public async Task<IActionResult> GetUserById(int id)
	{
		var user = await _service.GetUserByIdAsync(id);
		if (user == null)
		{
			return NotFound();
		}
		return Ok(user);
	}

	[HttpPost(Name = "CreateUser")]
	[Authorize]
	public async Task<IActionResult> CreateUser([FromBody] UserCreateDto user)
	{
		if (user == null)
		{
			return BadRequest("Required parameters cannot be null");
		}

		if (user.Username == null || user.Username.Length < 3)
		{
			return BadRequest("Username must be at least 3 characters long");
		}

		if (Validations.IsValidEmail(user.Email) == false)
		{
			return BadRequest("Invalid email format");
		}

		if (user.Password == null || user.Password.Length < 8)
		{
			return BadRequest("Password must be at least 8 characters or longer");
		}

		var createdUser = await _service.CreateUserAsync(user);

		if (!createdUser.IsSuccess)
		{
			return BadRequest(createdUser.ErrorMessage);
		}

		return CreatedAtAction(nameof(GetUserById), new { id = createdUser.User!.UserId }, createdUser);
	}

	[HttpPut("{id}", Name = "UpdateUser")]
	[Authorize]
	public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto user)
	{
		if (user == null)
		{
			return BadRequest("Required parameters cannot be null");
		}

		if (user.Username == null || user.Username.Length < 3)
		{
			return BadRequest("Username must be at least 3 characters long");
		}

		if (Validations.IsValidEmail(user.Email) == false)
		{
			return BadRequest("Invalid email format");
		}

		var updatedUser = await _service.UpdateUserAsync(id, user);
		if (updatedUser == null)
		{
			return NotFound();
		}
		return Ok(updatedUser);
	}

	[HttpDelete("{id}", Name = "DeleteUser")]
	[Authorize]
	public async Task<IActionResult> DeleteUser(int id)
	{
		bool result = await _service.DeleteUserAsync(id);
		if (!result)
		{
			return NotFound();
		}
		return NoContent();
	}
}
