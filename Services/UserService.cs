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

	public UserService(LeaderBoardDbContext dbcontext)
	{
		_dbcontext = dbcontext;
	}

	public async Task<(bool IsSuccess, User? User, string? ErrorMessage)> CreateUserAsync(UserCreateDto dto)
	{
		var existingUser = await _dbcontext.users.FirstOrDefaultAsync(u => u.Username == dto.Username);
		if (existingUser != null)
		{
			return (false, null, "User already exists");
		}

		string existingPassword = dto.Password;
		byte[] salt = Encoding.UTF8.GetBytes(dto.Username);
		string hashPassword = HashingHelper.HashPassword(existingPassword);
		var user = new User
		{
			Username = dto.Username,
			Email = dto.Email,
			Password = hashPassword,
			CreatedAt = DateTime.UtcNow
		};

		_dbcontext.Add(user);
		await _dbcontext.SaveChangesAsync();

		return (true, user, null);
	}
	public async Task<IEnumerable<User>> GetAllUsersAsync()
	{
		return await _dbcontext.users.ToListAsync();
	}
	public async Task<User?> GetUserByIdAsync(int id)
	{
		return await _dbcontext.users.FindAsync(id) ?? null;
	}

	public async Task<User?> UpdateUserAsync(int id, UserUpdateDto dto)
	{
		var existingUser = await _dbcontext.users.FindAsync(id);

		if (existingUser == null) { return null; }

		existingUser.Username = dto.Username;
		existingUser.Email = dto.Email;

		await _dbcontext.SaveChangesAsync();
		return existingUser;
	}
	public async Task<bool> DeleteUserAsync(int id)
	{
		var existingUser = _dbcontext.users.Find(id);

		if (existingUser == null) { return false; }

		_dbcontext.Remove(existingUser);
		await _dbcontext.SaveChangesAsync();
		return true;
	}
}
