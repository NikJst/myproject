using Microsoft.AspNetCore.Http;

namespace Testing3;

public interface IGuestService
{
    User GetOrCreateGuest(HttpContext context);
}
public class GuestService : IGuestService
{
    private readonly ILogger<GuestService> logger;
    private readonly IUserAndGuestRepository _userAndGuestRepository;

    public GuestService(ILogger<GuestService> logger, IUserAndGuestRepository userAndGuestRepository)
    {
        this.logger = logger;
        _userAndGuestRepository = userAndGuestRepository;
    }
    public User GetOrCreateGuest(HttpContext context)
    {
        // Проверяем, есть ли cookie
        var cookie = context.Request.Cookies["GuestId"]; //извлекаем строку с id гостя
        if (cookie != null && Guid.TryParse(cookie, out var userId))
        {
            logger.LogInformation($"Проверка существования пользователя с id {userId}");
            var existing = _userAndGuestRepository.Get(userId);
            if (existing != null)
                return existing; // если гость уже существует, возвращаем его объект
        }

        var guest = new Guest
        {
            GuidId = Guid.NewGuid(),
            Name = "Guest_" + Guid.NewGuid().ToString().Substring(0, 5)
        };

        _userAndGuestRepository.Add(guest);
        context.Response.Cookies.Append("GuestId", guest.GuidId.ToString());
        logger.LogInformation($"Пользователь не найден: создаем нового с id {guest.GuidId} и cookie, и отдаем его клиенту");

        return guest;
    }
}
