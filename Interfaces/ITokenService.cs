using System.Security.Claims;
using SafeProjectName.Models;

namespace SafeProjectName.Interfaces;

public interface ITokenService
{
	string GenerateAccessToken(IEnumerable<Claim> claims);
	string GenerateRefreshToken();
	ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
}
