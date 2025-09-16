using System.Text;
using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Services;
public class UserService : IUserService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ILogger<UserService> _logger;

	public UserService(LeaderBoardDbContext dbcontext, ILogger<UserService> logger)
	{
		_dbcontext = dbcontext;
		_logger = logger;
	}

	public async Task CreateUserAsync(UserCreateRequest request)
	{
		try
		{
			var existingUser = await _dbcontext.users.FirstOrDefaultAsync(u => u.Username == request.Username);
			if (existingUser != null)
			{
				throw new InvalidOperationException("User already exists");
			}

			string existingPassword = request.Password;
			byte[] salt = Encoding.UTF8.GetBytes(request.Username);
			string hashPassword = HashingHelper.HashPassword(existingPassword);
			var user = new User
			{
				Username = request.Username,
				Email = request.Email,
				Password = hashPassword,
				CreatedAt = DateTime.UtcNow
			};

			_dbcontext.Add(user);
			await _dbcontext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while creating a user");
			throw;
		}
	}

	public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
	{
		try
		{
			var users = await _dbcontext.users.ToListAsync();
			if (users == null || users.Count == 0)
			{
				throw new InvalidDataException("No user items found");
			}

			var response = users.Select(u => new UserResponse
			{
				UserId = u.UserId,
				Username = u.Username,
				Email = u.Email,
				CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
			}).ToList();

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving all users");
			throw;
		}
	}

	public async Task<UserResponse> GetUserByIdAsync(int id)
	{
		try
		{
			var user = await _dbcontext.users
			.Where(u => u.UserId == id)
			.Select(u => new UserResponse
			{
				UserId = u.UserId,
				Username = u.Username,
				Email = u.Email,
				CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
				Scores = u.Scores != null
					? u.Scores.Select(s => new ScoreResponse
					{
						ScoreId = s.ScoreId,
						Value = s.Value,
						AchievedAt = s.AchievedAt.ToString("yyyy-MM-dd HH:mm:ss"),
						UserId = s.UserId,
						GameId = s.GameId
					}).ToArray()
					: Array.Empty<ScoreResponse>()
			})
			.FirstOrDefaultAsync();

			if (user == null)
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			return user;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving a user by ID");
			throw;
		}
	}

	public async Task UpdateUserAsync(int id, UserUpdateRequest request)
	{
		try
		{
			var existingUser = await _dbcontext.users.FindAsync(id);
			if (existingUser == null)
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			existingUser.Username = request.Username ?? existingUser.Username;
			existingUser.Email = request.Email ?? existingUser.Email;

			await _dbcontext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while updating a user");
			throw;
		}
	}

	public async Task DeleteUserAsync(int id)
	{
		try
		{
			var existingUser = _dbcontext.users.Find(id);
			if (existingUser == null)
			{
				throw new KeyNotFoundException("No user found with the given ID");
			}

			_dbcontext.Remove(existingUser);
			await _dbcontext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while deleting a user");
			throw;
		}
	}
}
