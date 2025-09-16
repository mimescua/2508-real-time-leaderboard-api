using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
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

	[HttpGet("GetAllUsers", Name = "GetAllUsers")]
	[Authorize]
	public async Task<IActionResult> GetAllUsers()
	{
		var users = await _service.GetAllUsersAsync();
		return Ok(new { message = "Users retrieved successfully", data = users });
	}

	[HttpGet("{id}", Name = "GetUserById")]
	[Authorize]
	public async Task<IActionResult> GetUserById(int id)
	{
		var user = await _service.GetUserByIdAsync(id);
		return Ok(new { message = $"User {id} retrieved successfully", data = user });
	}

	[HttpPost("CreateUser", Name = "CreateUser")]
	[Authorize]
	public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest user)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(user.Username) || user.Username.Length < 3)
			{
				throw new BadHttpRequestException("Invalid Username", new Exception("Username must be at least 3 characters long"));
			}

			if (Validations.IsValidEmail(user.Email) == false)
			{
				throw new BadHttpRequestException("Invalid Email Format", new Exception("The provided email format is not valid"));
			}

			if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 8)
			{
				throw new BadHttpRequestException("Invalid Password", new Exception("Password must be at least 8 characters or longer"));
			}

			await _service.CreateUserAsync(user);
			return Ok(new { message = "User successfully created" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Invalid user creation parameters");
			throw;
		}
	}

	[HttpPut("{id}", Name = "UpdateUser")]
	[Authorize]
	public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest user)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(user.Username) && string.IsNullOrWhiteSpace(user.Email))
			{
				throw new BadHttpRequestException("Invalid parameters", new Exception("Username and Email cannot both be empty"));
			}

			if (!string.IsNullOrWhiteSpace(user.Username) && user.Username.Length < 3)
			{
				throw new BadHttpRequestException("Invalid Username", new Exception("Username must be at least 3 characters long"));
			}

			if (!string.IsNullOrWhiteSpace(user.Email) && Validations.IsValidEmail(user.Email) == false)
			{
				throw new BadHttpRequestException("Invalid Email Format", new Exception("The provided email format is not valid"));
			}

			await _service.UpdateUserAsync(id, user);
			return Ok(new { message = $"User {id} successfully updated" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Invalid for user update parameters");
			throw;
		}
	}

	[HttpDelete("{id}", Name = "DeleteUser")]
	[Authorize]
	public async Task<IActionResult> DeleteUser(int id)
	{
		await _service.DeleteUserAsync(id);
		return Ok(new { message = $"User {id} successfully deleted" });
	}
}
