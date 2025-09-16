using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeProjectName.Models;
using SafeProjectName.Models.Enums;

namespace SafeProjectName.DataAccess.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
	public void Configure(EntityTypeBuilder<Game> entity)
	{
		entity.HasKey(g => g.GameId);

		entity.Property(g => g.Title)
			.IsRequired()
			.HasMaxLength(100);

		entity.Property(g => g.About)
			.IsRequired(false)
			.HasMaxLength(500);

		entity.Property(g => g.Platforms)
			.IsRequired(false)
			.HasMaxLength(100);

		entity.Property(g => g.Genres)
					.HasConversion<int>()
					.IsRequired();

		entity.Property(g => g.ReleaseDate)
			.HasColumnType("datetime")
			.IsRequired();

		entity.HasMany(g => g.Scores)
			.WithOne(s => s.Game)
			.HasForeignKey(s => s.GameId)
			.OnDelete(DeleteBehavior.Cascade);

		entity.HasData(
			new Game
			{
				GameId = 1,
				Title = "The Legend of Zelda: Ocarina of Time",
				About = "An action-adventure game set in the fantasy land of Hyrule.",
				Platforms = "Nintendo 64, GameCube, iQue player",
				Genres = GameGenre.Action | GameGenre.Adventure | GameGenre.Puzzle | GameGenre.RolePlaying | GameGenre.Fighting,
				ReleaseDate = new DateTime(1998, 11, 21),
			},
			new Game
			{
				GameId = 2,
				Title = "Grand Theft Auto V",
				About = "An open-world action-adventure game set in the fictional state of San Andreas.",
				Platforms = "PlayStation 3, PlayStation 4, PlayStation 5, Xbox 360, Xbox One, PC",
				Genres = GameGenre.Action | GameGenre.Adventure | GameGenre.Shooter | GameGenre.Racing | GameGenre.RolePlaying | GameGenre.Simulation,
				ReleaseDate = new DateTime(2013, 9, 17),
			}
		);
	}
}
