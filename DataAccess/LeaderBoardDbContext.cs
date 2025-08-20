using Microsoft.EntityFrameworkCore;
using SafeProjectName.Models;

namespace SafeProjectName.DataAccess;

public class LeaderBoardDbContext : DbContext
{
	public DbSet<User> users { get; set; }
	public DbSet<Game> games { get; set; }
	public DbSet<Score> scores { get; set; }
	public DbSet<TokenInfo> tokenInfos { get; set; }

	public LeaderBoardDbContext(DbContextOptions<LeaderBoardDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeaderBoardDbContext).Assembly);
	}
}
