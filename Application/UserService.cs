using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Testing3.DTO;
namespace Testing3;

public interface IUserService
{
    Task<User> GetOrCreateUser(HttpContext context);
    Guid? GetUserIdFromCookie(HttpContext context);
    // Task<ViewUsersListDto> GetAllUsers();
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
            .FirstOrDefaultAsync(u => u.Id == userId);
            if (findUser != null)
                return findUser;
            throw new Exception("===> User not found");
        }


        var newUserId = Guid.NewGuid();
        var user = new User(string.Empty)
        {
            Id = newUserId,//одинаковые id 
            Name = newUserId.ToString(), // одинаковые id 
            Username = "User_" + newUserId.ToString()[..Math.Min(5, newUserId.ToString().Length)],
            IsGuest = true,
            CreatedAt = DateTime.UtcNow
        };

        await dbcontext.Users.AddAsync(user);
        await dbcontext.SaveChangesAsync();
        httpcontext.Response.Cookies.Append("GuestId", user.Id.ToString());
        logger.LogInformation("Создаем нового и отдаем cookie клиенту");

        return user; //целый обьект для использования в других методах


    }

    public Guid? GetUserIdFromCookie(HttpContext httpcontext)
    {
        var cookie = httpcontext.Request.Cookies["GuestId"];
        if (cookie != null && Guid.TryParse(cookie, out var userId))
        {
            return userId;
        }
        return null;
    }
}
