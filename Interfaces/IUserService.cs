using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface IUserService
{
	Task CreateUserAsync(UserCreateRequest request);
	Task<IEnumerable<UserResponse>> GetAllUsersAsync();
	Task<UserResponse> GetUserByIdAsync(int id);
	Task UpdateUserAsync(int id, UserUpdateRequest request);
	Task DeleteUserAsync(int id);
}
