namespace Testing3;
public interface IUserAndGuestRepository
{
    User? Get(Guid id);
    void Add(User user);
}
public class UserAndGuestRepository : IUserAndGuestRepository
{
    static Dictionary<Guid, User> Users = new Dictionary<Guid, User>();

    public User? Get(Guid userid)
    {
        Users.TryGetValue(userid, out var user);
        return user;//возвращаем пользователя или null
    }
    public void Add(User user)
    {

        Users[user.GuidId] = user;//добавляем пользователя в словарь
    }

    // // Отправляем cookie клиенту
    // context.Response.Cookies.Append("GuestId", guestId.ToString());

    // return guest;
}
