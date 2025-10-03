using StackExchange.Redis;

namespace SafeProjectName.Interfaces;

public interface ICacheService
{
	Task<bool> AddOrUpdateSortedSetAsync(string key, string member, double score);
	Task<long> AddOrUpdateRangeToSortedSetAsync(string key, (string member, double score)[] values);
	Task<long> CountSortedSetAsync(string key);
	Task<double> GetSingleSortedSetScoreAsync(string key, string member);
	Task<(double? score, long? rank)> GetSingleSortedSetAsync(string key, string member, bool descending = true);
	Task<List<(string member, double score)>> GetTopXSortedSetAsync(string key, int count);
	Task<SortedSetEntry[]> GetSortedSetRangeAsync(string key, int pageNumber, int pageSize);

	Task SetAsync<T>(string key, T value, TimeSpan expiry) where T : class;
	Task<T?> GetAsync<T>(string key) where T : class;
	Task<bool> IsSetAsync(string key);
	Task ClearAsync(string key);
	Task ClearKeysByPatternAsync(string pattern);
}
