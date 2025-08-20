using SafeProjectName.Models;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface IUserService
{
	Task<(bool IsSuccess, User? User, string? ErrorMessage)> CreateUserAsync(UserCreateDto dto);
	Task<IEnumerable<User>> GetAllUsersAsync();
	Task<User?> GetUserByIdAsync(int id);
	Task<User?> UpdateUserAsync(int id, UserUpdateDto dto);
	Task<bool> DeleteUserAsync(int id);
}
