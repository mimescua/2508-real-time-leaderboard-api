using Microsoft.EntityFrameworkCore;
using SafeProjectName.Models;

namespace SafeProjectName.DataAccess;

public class LeaderBoardDbContext : DbContext
{
	public DbSet<User> users { get; set; } = null!;
	public DbSet<Game> games { get; set; } = null!;
	public DbSet<Score> scores { get; set; } = null!;
	public DbSet<TokenInfo> tokenInfos { get; set; } = null!;

	public LeaderBoardDbContext(DbContextOptions<LeaderBoardDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeaderBoardDbContext).Assembly);
	}
}
