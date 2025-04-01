using Microsoft.Extensions.Caching.Distributed;

namespace Stockas.Application.Services.Token
{
    public class RedisTokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisTokenBlacklistService> _logger;

        public RedisTokenBlacklistService(
            IDistributedCache cache,
            ILogger<RedisTokenBlacklistService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task BlacklistTokenAsync(string token, TimeSpan expiry)
        {
            try
            {
                _logger.LogInformation("Blacklisting token");
                var cacheKey = GetCacheKey(token);
                await _cache.SetStringAsync(
                    cacheKey,
                    "blacklisted",
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to blacklist token");
                throw;
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            try
            {
                _logger.LogInformation("Checking if token is blacklisted");
                var cacheKey = GetCacheKey(token);
                var result = await _cache.GetStringAsync(cacheKey);
                return result != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check token blacklist");
                throw;
            }
        }

        private static string GetCacheKey(string token) => $"blacklisted_token:{token}";
    }
}
