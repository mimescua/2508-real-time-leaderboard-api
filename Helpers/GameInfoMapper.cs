using SafeProjectName.Models.Enums;

namespace SafeProjectName.Helpers;

public static class GameInfoMapper
{
	public static string[] GetGenreNames(GameGenre genreValue)
	{
		var matchedGenres = new List<string>();
		ushort remainingBits = (ushort)genreValue;

		// Sort by number of bits set (composite flags first)
		var sortedGenres = Enum.GetValues(typeof(GameGenre))
			.Cast<GameGenre>()
			.Where(g => g != GameGenre.None)
			.OrderByDescending(g => CountBits((ushort)g));

		foreach (var genre in sortedGenres)
		{
			ushort genreBits = (ushort)genre;
			if ((remainingBits & genreBits) == genreBits)
			{
				matchedGenres.Add(genre.ToString());
				remainingBits &= (ushort)~genreBits; // Remove matched bits
			}
		}

		return matchedGenres.ToArray();
	}

	private static int CountBits(ushort value)
	{
		int count = 0;
		while (value != 0)
		{
			count += value & 1;
			value >>= 1;
		}
		return count;
	}
}
