using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Testing3.DTO;
namespace Testing3;

public interface IUserService
{
    Task<User> GetOrCreateUser(HttpContext context);
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
            .FirstOrDefaultAsync(u => u.UserId == userId);
            if (findUser != null)
                return findUser;
        }

        var newUserId = Guid.NewGuid();
        var user = new User(string.Empty)
        {
            UserId = newUserId,//одинаковые id 
            Name = newUserId.ToString(), // одинаковые id 
            Username = "User_" + newUserId.ToString().Substring(0, Math.Min(5, newUserId.ToString().Length)),
            IsGuest = true,
            CreatedAt = DateTime.UtcNow
        };

        await dbcontext.Users.AddAsync(user);
        await dbcontext.SaveChangesAsync();
        httpcontext.Response.Cookies.Append("GuestId", user.UserId.ToString());
        logger.LogInformation("Создаем нового и отдаем cookie клиенту");

        return user; //целый обьект для использования в других методах

    }

    //----не помню делал ли я сам 
    /* public async Task<ViewUsersListDto> GetAllUsers()
     {
         var users = await dbcontext.Users
         .OrderBy(u => u.Name)//нет индекса по имени, но в тесте не жалко
         .ToListAsync();
         var count = await dbcontext.Users.CountAsync();
         logger.LogInformation($"Получено {count} пользователей");

         // Преобразуем User в ViewUserCardDto
         var userCards = users.Select(u => new ViewUserCardDto
         {
             UserId = u.UserId,
             Username = u.Username,
             Name = u.Name,
             Description = u.Description,
             PostCount = u.PostCount,
             LikeCount = u.LikeCount,
             CreatedAt = u.CreatedAt,
             IsOnline = u.IsOnline
         }).ToList();

         return new ViewUsersListDto
         {
             Users = userCards,
             UsersCount = count
         };
     }
 }*/
}
