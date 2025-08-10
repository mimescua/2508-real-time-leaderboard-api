using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeProjectName.Models;

namespace SafeProjectName.DataAccess.Configurations;

public class ScoreConfiguration : IEntityTypeConfiguration<Score>
{
	public void Configure(EntityTypeBuilder<Score> entity)
	{
		entity.HasKey(g => g.ScoreId);

		entity.Property(g => g.Value)
			.IsRequired();

		entity.Property(s => s.AchievedAt)
			.HasColumnType("datetime")
			.IsRequired();

		entity.HasOne(s => s.User)
			.WithMany(u => u.Scores)
			.HasForeignKey(s => s.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		entity.HasOne(s => s.Game)
			.WithMany(g => g.Scores)
			.HasForeignKey(s => s.GameId)
			.OnDelete(DeleteBehavior.Cascade);

		entity.HasIndex(s => new { s.UserId, s.GameId })
			.IsUnique();

		entity.HasData(
			new Score
			{
				ScoreId = 1,
				UserId = 1,
				GameId = 1,
				Value = 100,
				AchievedAt = new DateTime(2025, 10, 1)
			},
			new Score
			{
				ScoreId = 2,
				UserId = 1,
				GameId = 2,
				Value = 900,
				AchievedAt = new DateTime(2025, 10, 2)
			},
			new Score
			{
				ScoreId = 3,
				UserId = 2,
				GameId = 2,
				Value = 600,
				AchievedAt = new DateTime(2025, 10, 3)
			}
		);
	}
}
