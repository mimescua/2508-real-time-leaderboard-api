using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Middleware;
using SafeProjectName.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("LeaderBoardConnection");
builder.Services.AddDbContext<LeaderBoardDbContext>(options => options.UseSqlServer(connectionString));

var redisOptions = ConfigurationOptions.Parse(
	builder.Configuration.GetConnectionString("RedisConnection")
	?? throw new InvalidOperationException("Redis connection string not found.")
);
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IHistoricalService, HistoricalService>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddLogging();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	using (var scope = app.Services.CreateScope())
	{
		var dbcontext = scope.ServiceProvider.GetRequiredService<LeaderBoardDbContext>();
		dbcontext.Database.Migrate();

		var redisdbcontext = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
		_ = redisdbcontext.Ping();
		await new RedisSeedService().SeedLeaderboardAsync(redisdbcontext);
	}

	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
