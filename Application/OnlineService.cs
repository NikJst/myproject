using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Caching.Memory;

namespace Testing3.Application;

public interface IOnlineService
{
    // Task<bool> IsUserOnlineAsync(string userId);
    void SetUserOnline(string userId);
    bool IsUserOnline(string userId);
    Dictionary<string, bool> GetOnlineUsers(IEnumerable<string> postIds);
    // Task<IEnumerable<string>> GetOnlineUsersAsync();
}

public class OnlineService : IOnlineService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<OnlineService> _logger;
    // private readonly string _key = "online status";
    public OnlineService(IMemoryCache cache, ILogger<OnlineService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public void SetUserOnline(string userId)
    {
        var options = new MemoryCacheEntryOptions()
             .SetAbsoluteExpiration(TimeSpan.FromSeconds(10)); // Удалить через 10 сек автоматически
        _logger.LogInformation("User {UserId} is online at {Time}", userId, DateTime.UtcNow);
        _cache.Set(userId, true, options); // true - пользователь онлайн; options - настройки кэша
        _logger.LogInformation("User {UserId} cached as online", userId);
    }

    public bool IsUserOnline(string userId)
    {
        return _cache.TryGetValue(userId, out _); //out_ - не используем значение, только проверяем наличие
    }

    public Dictionary<string, bool> GetOnlineUsers(IEnumerable<string> postIds)
    {
        var result = new Dictionary<string, bool>();

        foreach (var postId in postIds)
        {
            // Проверяем онлайн статус для пользователя связанного с постом
            // Используем postId как userId (предполагаем что они совпадают)
            var isOnline = _cache.TryGetValue(postId, out _);
            result[postId] = isOnline;
            _logger.LogInformation("User {UserId} online status: {IsOnline}", postId, isOnline);
        }
        return result;
    }
}
