using Microsoft.EntityFrameworkCore;
using SafeProjectName.DataAccess;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("LeaderBoardConnection");
builder.Services.AddDbContext<LeaderBoardDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
