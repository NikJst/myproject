
using Microsoft.AspNetCore.Http;

namespace Testing3;
public class GuestRepository : IGuestService
{
    static Dictionary<Guid, User> Users = new Dictionary<Guid, User>();


    public User GetOrCreateGuest(HttpContext context)// — объект, который содержит всю информацию о текущем HTTP-запросе и ответе:
    {
        // Проверяем, есть ли cookie
        var cookie = context.Request.Cookies["GuestId"];
        Guid guestId;
        if (cookie != null && Guid.TryParse(cookie, out guestId) && Users.ContainsKey(guestId))
            return Users[guestId];

        // Создаём нового гостя
        guestId = Guid.NewGuid();
        var guest = new User
        {
            GuidId = guestId,
            Name = "Guest_" + guestId.ToString().Substring(0, 5)
        };
        Users[guestId] = guest; // Добавляем в хранилище гостя 

        // Отправляем cookie клиенту
        context.Response.Cookies.Append("GuestId", guestId.ToString());

        return guest;
    }
}