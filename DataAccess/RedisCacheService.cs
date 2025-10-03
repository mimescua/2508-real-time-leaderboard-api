using System.Text.Json;
using SafeProjectName.Interfaces;
using StackExchange.Redis;

namespace SafeProjectName.DataAccess;

public class RedisCacheService : ICacheService
{
	private readonly IConnectionMultiplexer _redisconnection;
	private readonly IDatabase _dbcontext;
	private readonly JsonSerializerOptions _jsonOptions;

	public RedisCacheService(IConnectionMultiplexer redis)
	{
		_redisconnection = redis ?? throw new ArgumentNullException(nameof(redis));
		_dbcontext = _redisconnection.GetDatabase();

		_jsonOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
		};
	}

	public async Task<bool> AddOrUpdateSortedSetAsync(string key, string member, double score)
	{
		return await _dbcontext.SortedSetAddAsync(key, member, score);
	}

	public async Task<long> AddOrUpdateRangeToSortedSetAsync(string key, (string member, double score)[] values)
	{
		var entries = new SortedSetEntry[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			entries[i] = new SortedSetEntry(values[i].member, values[i].score);
		}

		return await _dbcontext.SortedSetAddAsync(key, entries);
	}

	public async Task<long> CountSortedSetAsync(string key)
	{
		return await _dbcontext.SortedSetLengthAsync(key);
	}

	public async Task<double> GetSingleSortedSetScoreAsync(string key, string member)
	{
		double? score = await _dbcontext.SortedSetScoreAsync(key, member);
		return score ?? 0;
	}

	public async Task<(double? score, long? rank)> GetSingleSortedSetAsync(string key, string member, bool descending = true)
	{
		double? score = await _dbcontext.SortedSetScoreAsync(key, member);
		if (score == null)
		{
			return (null, null);
		}

		long? rank = await _dbcontext.SortedSetRankAsync(key, member, descending ? Order.Descending : Order.Ascending);
		return (score, rank);
	}

	public async Task<List<(string member, double score)>> GetTopXSortedSetAsync(string key, int count)
	{
		var results = await _dbcontext.SortedSetRangeByRankWithScoresAsync(key, 0, count - 1, Order.Descending);

		return results.Select(r => (r.Element.ToString(), r.Score)).ToList();
	}

	public async Task<SortedSetEntry[]> GetSortedSetRangeAsync(string key, int pageNumber, int pageSize)
	{
		int start = (pageNumber - 1) * pageSize;
		int stop = start + pageSize - 1;

		var results = await _dbcontext.SortedSetRangeByRankWithScoresAsync(key, start, stop, Order.Descending);
		return results;
	}

	public async Task<(double? score, long? rank)> GetMemberAsync(string key, string member, bool descending = true)
	{
		double? score = await _dbcontext.SortedSetScoreAsync(key, member);
		long? rank = await _dbcontext.SortedSetRankAsync(key, member, descending ? Order.Descending : Order.Ascending);

		return (score, rank);
	}





	public async Task SetAsync<T>(string key, T value, TimeSpan expiry) where T : class
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			throw new ArgumentException("Key cannot be null or empty", nameof(key));
		}

		string json = JsonSerializer.Serialize(value, _jsonOptions);
		await _dbcontext.StringSetAsync(key, json, expiry);
	}

	public async Task<T?> GetAsync<T>(string key) where T : class
	{
		Console.WriteLine("Fetching from Redis Cache");
		if (string.IsNullOrWhiteSpace(key))
		{
			return null;
		}

		Console.WriteLine(key);
		var value = await _dbcontext.StringGetAsync(key);
		Console.WriteLine(value);
		if (value.IsNullOrEmpty)
		{
			return null;
		}

		try
		{
			Console.WriteLine(JsonSerializer.Deserialize<T>(value!, _jsonOptions));
			return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
		}
		catch
		{
			// log deserialization error
			return null;
		}
	}

	public async Task<bool> IsSetAsync(string key)
	{
		return await _dbcontext.KeyExistsAsync(key);
	}

	public async Task ClearAsync(string key)
	{
		await _dbcontext.KeyDeleteAsync(key);
	}

	public async Task ClearKeysByPatternAsync(string pattern)
	{
		var endpoint = _redisconnection.GetEndPoints().First();
		var server = _redisconnection.GetServer(endpoint);

		// WARNING: expensive on large datasets
		var keys = server.Keys(pattern: pattern).ToArray();

		if (keys.Length > 0)
		{
			await _dbcontext.KeyDeleteAsync(keys);
		}
	}

	// Implement a method to save Redis Sorted Sets


	// private readonly IConfiguration _configuration;
	// private readonly ConnectionMultiplexer _redisconnection;
	// private readonly IDatabase _dbcontext;

	// public RedisCacheService(IConfiguration configuration)
	// {
	// 	_configuration = configuration;

	// 	string? host = _configuration["RedisConfig:Host"];
	// 	string? port = _configuration["RedisConfig:Port"];
	// 	string? password = _configuration["RedisConfig:Password"];

	// 	var options = new ConfigurationOptions
	// 	{
	// 		EndPoints = { $"{host}:{port}" },
	// 		Password = password,
	// 		AbortOnConnectFail = false
	// 	};

	// 	_redisconnection = ConnectionMultiplexer.Connect(options);
	// 	_dbcontext = _redisconnection.GetDatabase();
	// }

	// public void Set<T>(string key, T value, TimeSpan time) where T : class
	// {
	// 	string json = JsonSerializer.Serialize(value);
	// 	_dbcontext.StringSet(key, json, time);
	// }

	// public T? Get<T>(string key) where T : class
	// {
	// 	var value = _dbcontext.StringGet(key);
	// 	return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<T>(value);
	// }

	// public bool IsSet(string key)
	// {
	// 	bool value = _dbcontext.KeyExists(key);
	// 	return value;
	// }

	// public void Clear(string key)
	// {
	// 	_dbcontext.KeyDelete(key);
	// }

	// public void ClearKeysByPattern(string pattern)
	// {
	//     var endpoint = _redisconnection.GetEndPoints().First();
	//     var server = _redisconnection.GetServer(endpoint);

	//     var keys = server.Keys(pattern: pattern).Select(k => (string)k).ToList();

	//     if (keys != null && keys.Count > 0)
	//     {
	//         foreach (string? key in keys)
	//         {
	//             Clear(key);
	//         }
	//     }
	// }
}
