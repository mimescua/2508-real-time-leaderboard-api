using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Interfaces;

public interface IAccountService
{
	Task<(bool IsSuccess, TokenDto? token, string? ErrorMessage)> LoginAsync(string username, string password);
	Task<bool> LogoutAsync(string accessToken);
	Task<(bool IsSuccess, TokenDto? token, string? ErrorMessage)> RefreshTokenAsync(TokenDto currentToken);
}
