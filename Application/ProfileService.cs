using Testing3.DTO;
using Microsoft.EntityFrameworkCore;
namespace Testing3.Application;

public interface IProfileService
{
    Task<ProfileInfoDto> PatchInfo(ProfileInfoDto profileDto, User user);
    Task<ProfileInfoDto> GetProfileInfo(string username, User user);
    Task<List<UserPostDto>> GetUserPosts(string username, User user);
    // Task<ProfileDto> GetUserProfileContent(Guid userId);
}

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<ProfileService> logger;
    public ProfileService(ApplicationDbContext dbContext, ILogger<ProfileService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }
    public async Task<ProfileInfoDto> PatchInfo(ProfileInfoDto profileDto, User user)
    {

        var userinfo = await dbContext.Users.FindAsync(user);
        if (userinfo != null)
        {
            userinfo.Username = profileDto.Username;
            userinfo.Header = profileDto.Header;
            userinfo.Description = profileDto.Description;
            await dbContext.SaveChangesAsync();

            logger.LogWarning($"Profile обновлен в бд");
            var profileInfo = new ProfileInfoDto
            {
                UserId = userinfo.UserId,
                Username = userinfo.Username,
                Header = userinfo.Header,
                Description = userinfo.Description,
                /* 
                тут добавить поля которые хотим изменить
                */
            };
            logger.LogWarning($"Profile обновлен в бд");
            return profileInfo;
        }
        throw new Exception("Ошибка обновления профиля");
    }


    //-------->
    public async Task<ProfileInfoDto> GetProfileInfo(string username, User user)
    // таким образом получается что у нас один объект ProfileDto для всех операций, но с разным содержимым
    {
        var usernameUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username);

        if (usernameUser != null)
        {

            if (user.UserId == usernameUser.UserId) // если это наш профиль
            {
                // Считаем посты пользователя
                var postCountMe = await dbContext.Posts
                .Where(p => p.UserId == user.UserId)
                .CountAsync();

                var userProfileInfo = new ProfileInfoDto
                {
                    UserId = user.UserId,// тебе нужен
                    Username = user.Username,
                    Header = user.Header,
                    Description = user.Description,
                    PostCount = postCountMe
                    /* 
                    тут можно добавить другие поля, которые нужны только для нашего профиля
                    Например: 
                    счетчики подписчиков, подписок, лайков и т.д.

                    также поля для управления (редактирование, удаление и т.д.)
                    */
                };
                logger.LogWarning($"Profile получен для нашего профиля");
                return userProfileInfo;
            }

            // Считаем посты для отображения на чужом профиле
            var postCountAlien = await dbContext.Posts
            .Where(p => p.UserId == usernameUser.UserId)
            .CountAsync();

            var alienProfileInfo = new ProfileInfoDto
            {
                // тебе не нужны эти данные для чужого профиля
                // UserId = usernameUser.UserId,
                Username = usernameUser.Username,
                Header = usernameUser.Header,
                Description = usernameUser.Description,
                /* 
                тут можно добавить другие поля чужого профиля
                */
                PostCount = postCountAlien
            };
            logger.LogWarning($"Profile получен для чужого профиля");
            return alienProfileInfo;
        }
        else
        {
            logger.LogWarning("Владелец профиля - {username} не найден", username);
            throw new Exception("Владелец профиля не найден");
        }
    }

    //--------------->  получение собственных постов пользователя
    public async Task<List<UserPostDto>> GetUserPosts(string username, User user)
    {
        var usernameUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username);

        if (usernameUser != null)
        {
            var userPosts = await dbContext.Posts //при обращении к бд он уже коллекция
            .Where(p => p.UserId == usernameUser.UserId)
            .Select(p => new UserPostDto
            {
                UserId = p.UserId,
                Username = usernameUser.Username,
                PostId = p.PostId,
                Text = p.Text,
                Title = p.Title,
                LikedByUser = dbContext.Likes.Any(l => l.PostId == p.PostId && l.UserId == user.UserId),
                // IsGuest = false
                // CreatedAt = p.CreatedAt
            })
            .ToListAsync(); //оборачиваем в список
            logger.LogWarning($"Посты пользователя ({username}) получены");
            return userPosts;
        }
        else
        {
            logger.LogWarning("Владелец профиля - {username} не найден", username);
            throw new Exception("Владелец профиля не найден");
        }
    }
}
