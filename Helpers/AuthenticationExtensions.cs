using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SafeProjectName.Helpers;

public static class AuthenticationExtensions
{
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.SaveToken = true;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidAudience = configuration["JWT:ValidAudience"],
				ValidIssuer = configuration["JWT:ValidIssuer"],
				ClockSkew = TimeSpan.Zero,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
			};
		});

		return services;
	}
}
