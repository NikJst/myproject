using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace Testing3;

public interface IUserService
{
    Task<User> GetOrCreateUser(HttpContext context);
    Task<List<User>> GetAllUsers();
}
public class UserService : IUserService
{
    private readonly ILogger<UserService> logger;
    private readonly ApplicationDbContext dbcontext;

    public UserService(ILogger<UserService> logger, ApplicationDbContext context)
    {
        this.logger = logger;
        dbcontext = context;
    }
    public async Task<User> GetOrCreateUser(HttpContext httpcontext)
    {
        // Проверяем, на наличие записи в cookie
        var cookie = httpcontext.Request.Cookies["GuestId"];
        if (cookie != null && Guid.TryParse(cookie, out var userId))
        {
            var findUser = await dbcontext.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
            if (findUser != null)
            {
                return findUser;
            }
        }

        var user = new User(string.Empty)
        {
            UserId = Guid.NewGuid(),
            Name = "Guest_" + Guid.NewGuid().ToString()[..5],
            IsGuest = true,
            CreatedAt = DateTime.UtcNow
        };

        await dbcontext.Users.AddAsync(user);
        await dbcontext.SaveChangesAsync();
        httpcontext.Response.Cookies.Append("GuestId", user.UserId.ToString());
        logger.LogInformation("Создаем нового и отдаем cookie клиенту");

        return user; //целый обьект для использования в других методах

    }


    public async Task<List<User>> GetAllUsers()
    {
        var users = await dbcontext.Users
        .OrderBy(u => u.Name)//нет индекса по имени, но в тесте не жалко
        .ToListAsync();
        var count = await dbcontext.Users.CountAsync();
        logger.LogInformation($"Получено {count} пользователей");
        return users;
    }
}
