namespace Testing3;
public interface IUserAndGuestRepository
{
    User? Get(Guid id);
    void Add(User user);
}
public class UserAndGuestRepository : IUserAndGuestRepository
{
    private readonly ILogger<UserAndGuestRepository> logger;
    public UserAndGuestRepository(ILogger<UserAndGuestRepository> logger)
    {
        this.logger = logger;
    }

    static Dictionary<Guid, User> Users = new Dictionary<Guid, User>();

    public User? Get(Guid userid)
    {
        Users.TryGetValue(userid, out var user);
        logger.LogInformation($"Пользователь найден {user?.Name} => return");
        return user;
    }
    public void Add(User user)
    {

        Users[user.GuidId] = user;//добавляем пользователя в словарь
    }
}
