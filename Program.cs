using System.Text;
using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;
using SafeProjectName.Helpers;
using SafeProjectName.Interfaces;
using SafeProjectName.Services;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("LeaderBoardConnection");
builder.Services.AddDbContext<LeaderBoardDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();

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
	}

	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
