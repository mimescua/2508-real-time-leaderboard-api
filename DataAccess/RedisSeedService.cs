using SafeProjectName.Constants;
using StackExchange.Redis;

namespace SafeProjectName.DataAccess;

public class RedisSeedService
{
	public async Task SeedLeaderboardAsync(IDatabase _db)
	{
		await _db.SortedSetAddAsync($"{ScoreKeys.GamePrefix}1", "admin", 100);

		if (!await _db.KeyExistsAsync($"{ScoreKeys.GamePrefix}2"))
		{
			var entries = new SortedSetEntry[]
			{
				new SortedSetEntry("admin", 900),
				new SortedSetEntry("user1", 600),
			};
			await _db.SortedSetAddAsync($"{ScoreKeys.GamePrefix}2", entries);
		}

		if (!await _db.KeyExistsAsync($"{ScoreKeys.UserPrefix}1"))
		{
			var entries = new SortedSetEntry[]
			{
				new SortedSetEntry("The Legend of Zelda: Ocarina of Time", 100),
				new SortedSetEntry("Grand Theft Auto V", 900),
			};
			await _db.SortedSetAddAsync($"{ScoreKeys.UserPrefix}1", entries);
		}

		await _db.SortedSetAddAsync($"{ScoreKeys.UserPrefix}2", "Grand Theft Auto V", 600);

		if (!await _db.KeyExistsAsync($"{ScoreKeys.GlobalPrefix}"))
		{
			var entries = new SortedSetEntry[]
			{
				new SortedSetEntry("admin", 1000),
				new SortedSetEntry("user1", 600),
			};
			await _db.SortedSetAddAsync($"{ScoreKeys.GlobalPrefix}", entries);
		}
	}
}
