using Microsoft.Extensions.Caching.Memory;

namespace Testing3.ApplicatOnline;

public interface IOnlineService
{
    // Task<bool> IsUserOnlineAsync(string userId);
    void SetUserOnline(string userId);
    bool IsUserOnline(string userId);
    // Task<IEnumerable<string>> GetOnlineUsersAsync();
}

public class OnlineService : IOnlineService
{
    private readonly IMemoryCache _cache;
    // private readonly string _key = "online status";
    public OnlineService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void SetUserOnline(string userId)
    {
        var options = new MemoryCacheEntryOptions()
             .SetAbsoluteExpiration(TimeSpan.FromSeconds(2)); // Удалить через 6 сек автоматически
        _cache.Set(userId, true, options); // true - пользователь онлайн; options - настройки кэша
    }

    public bool IsUserOnline(string userId)
    {
        return _cache.TryGetValue(userId, out _); //out_ - не используем значение, только проверяем наличие
    }
}