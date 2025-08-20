using Microsoft.OpenApi.Models;

namespace SafeProjectName.Helpers;

public static class SwaggerExtensions
{
	public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
	{
		services.AddSwaggerGen(options =>
		{
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "Enter the Bearer Authorization string as follows: `Bearer YOUR_TOKEN`",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					new string[] {}
				}
			});
		});

		return services;
	}
}
