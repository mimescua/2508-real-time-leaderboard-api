using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Models;
using SafeProjectName.Models.DTOs;

namespace SafeProjectName.Services;

public class AccountService : IAccountService
{
	private readonly LeaderBoardDbContext _dbcontext;
	private readonly ITokenService _tokenService;

	public AccountService(LeaderBoardDbContext dbcontext, ITokenService tokenService)
	{
		_dbcontext = dbcontext;
		_tokenService = tokenService;
	}

	public async Task<(bool IsSuccess, TokenDto? token, string? ErrorMessage)> LoginAsync(string username, string password)
	{
		var user = await _dbcontext.users.FirstOrDefaultAsync(u => u.Username == username);
		if (user == null)
		{
			return (false, null, "User is not registered");
		}

		bool isValidPassword = HashingHelper.VerifyPassword(password, user.Password);
		if (isValidPassword == false)
		{
			return (false, null, "Invalid password");
		}

		List<Claim> authClaims = [
			new (ClaimTypes.Name, user.Username),
			new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		];
		string token = _tokenService.GenerateAccessToken(authClaims);
		string refreshToken = _tokenService.GenerateRefreshToken();

		var tokenInfo = _dbcontext.tokenInfos.FirstOrDefault(a => a.Username == user.Username);
		if (tokenInfo == null)
		{
			var ti = new TokenInfo
			{
				Username = user.Username,
				RefreshToken = refreshToken,
				ExpiredAt = DateTime.UtcNow.AddDays(3)
			};
			_dbcontext.tokenInfos.Add(ti);
		}
		else
		{
			tokenInfo.RefreshToken = refreshToken;
			tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(3);
		}

		await _dbcontext.SaveChangesAsync();

		return (true, new TokenDto
		{
			AccessToken = token,
			RefreshToken = refreshToken
		}, null);
	}

	public async Task<bool> LogoutAsync(string accessToken)
	{
		var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
		string? username = principal.Identity!.Name;
		if (username == null)
		{
			return false;
		}

		var tokenInfo = _dbcontext.tokenInfos.SingleOrDefault(u => u.Username == username);
		if (tokenInfo == null)
		{
			return false;
		}

		tokenInfo.RefreshToken = string.Empty;
		await _dbcontext.SaveChangesAsync();

		return true;
	}

	public async Task<(bool IsSuccess, TokenDto? token, string? ErrorMessage)> RefreshTokenAsync(TokenDto tokenDTO)
	{
		var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDTO.AccessToken);
		string? username = principal.Identity!.Name;
		if (username == null)
		{
			return (false, null, "Invalid token");
		}

		var tokenInfo = _dbcontext.tokenInfos.SingleOrDefault(u => u.Username == username);
		if (tokenInfo == null
			|| tokenInfo.RefreshToken != tokenDTO.RefreshToken
			|| tokenInfo.ExpiredAt <= DateTime.UtcNow)
		{
			return (false, null, "Invalid refresh token. Please login again.");
		}

		string newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
		string newRefreshToken = _tokenService.GenerateRefreshToken();

		tokenInfo.RefreshToken = newRefreshToken;
		await _dbcontext.SaveChangesAsync();

		return (true, new TokenDto
		{
			AccessToken = newAccessToken,
			RefreshToken = newRefreshToken
		}, null);
	}
}
