using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeProjectName.Models;

namespace SafeProjectName.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> entity)
	{
		entity.HasKey(g => g.UserId);

		entity.HasIndex(g => g.Username)
			.IsUnique();

		entity.Property(g => g.Username)
			.IsRequired()
			.HasMaxLength(32);

		entity.Property(g => g.Email)
			.IsRequired()
			.HasMaxLength(320);

		entity.Property(g => g.Password)
			.IsRequired()
			.HasMaxLength(72);

		entity.Property(g => g.CreatedAt)
			.HasColumnType("datetime")
			.IsRequired();

		entity.HasMany(g => g.Scores)
			.WithOne(s => s.User)
			.HasForeignKey(s => s.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		entity.HasData(
			new User
			{
				UserId = 1,
				Username = "admin",
				Email = "admin@leaderboard.com",
				Password = "admin123",
				CreatedAt = new DateTime(2024, 1, 15)
			},
			new User
			{
				UserId = 2,
				Username = "user1",
				Email = "user1@leaderboard.com",
				Password = "user123",
				CreatedAt = new DateTime(2024, 10, 27)
			}
		);
	}
}
