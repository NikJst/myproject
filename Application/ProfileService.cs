using Testing3.DTO;
using Microsoft.EntityFrameworkCore;
namespace Testing3.Application;

public interface IProfileService
{
    Task<ProfileInfoDto> PatchInfo(ProfileInfoDto profileDto, User user);
    Task<ProfileInfoDto> GetTargetProfile(string username, User user);
    Task<PagedResponse<ViewPostsDto>> GetTargetUserPosts(string username, User user);
    // Task<ProfileDto> GetUserProfileContent(Guid userId);
    Task<List<ViewPostsDto>> GetLikesPosts(string username, User user);
    // Task<List<UserPostDto>> GetFavoritesPosts(string username, User user);
}


public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<ProfileService> logger;
    private readonly IOnlineService onlineService;

    public ProfileService(ApplicationDbContext dbContext, ILogger<ProfileService> logger, IOnlineService onlineService)
    {
        this.dbContext = dbContext;
        this.logger = logger;
        this.onlineService = onlineService;
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
    public async Task<ProfileInfoDto> GetTargetProfile(string username, User user)
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

                // Считаем созданные лайки пользователя
                var likesCountMe = await dbContext.Likes
                .Where(l => l.UserId == user.UserId)
                .CountAsync();

                var userProfileInfo = new ProfileInfoDto
                {
                    UserId = user.UserId,// тебе нужен
                    Username = user.Username,
                    Header = user.Header,
                    Description = user.Description,
                    IsOnline = onlineService.IsUserOnline(user.UserId.ToString()),
                    PostCount = postCountMe,
                    LikesCount = likesCountMe
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

            // Считаем созданные лайки чужого пользователя
            var likesCountAlien = await dbContext.Likes
            .Where(l => l.UserId == usernameUser.UserId)
            .CountAsync();

            var alienProfileInfo = new ProfileInfoDto
            {
                // тебе не нужны эти данные для чужого профиля
                // UserId = usernameUser.UserId,
                Username = usernameUser.Username,
                Header = usernameUser.Header,
                Description = usernameUser.Description,
                IsOnline = onlineService.IsUserOnline(usernameUser.UserId.ToString()),
                /* 
                тут можно добавить другие поля чужого профиля
                */
                PostCount = postCountAlien,
                LikesCount = likesCountAlien
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
    public async Task<PagedResponse<ViewPostsDto>> GetTargetUserPosts(string username, User user)
    {
        var targetUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username);

        if (targetUser != null)
        {
            var query = dbContext.Posts
            .Where(p => p.UserId == targetUser.UserId) //среди всех постов находим таргетный
            .Select(p => new ViewPostsDto
            {
                UserId = p.UserId,
                Username = targetUser.Username,
                PostId = p.PostId,
                Text = p.Text,
                Title = p.Title,
                LikedByUser = dbContext.Likes.Any(l => l.PostId == p.PostId && l.UserId == user.UserId),
                LikesCount = dbContext.Likes.Count(l => l.PostId == p.PostId)
            });
            var response = await query.Skip(0).Take(10).ToListAsync(); //оборачиваем в список
            logger.LogWarning($"Посты пользователя ({username}) получены");
            return new PagedResponse<ViewPostsDto> { Items = response, Meta = new MetaData() }; //в коллекцию передаем список постов и в будщем еще что то
        }
        else
        {
            logger.LogWarning("Владелец профиля - {username} не найден", username);
            throw new Exception("Владелец профиля не найден");
        }
    }

    public async Task<List<ViewPostsDto>> GetLikesPosts(string username, User user)
    {
        var usernameUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username);

        if (usernameUser != null)
        {
            var likedPosts = await dbContext.Likes //начинаем с таблицы лайков
            .Where(l => l.UserId == usernameUser.UserId) //лайки пользователя
            .Select(p => new ViewPostsDto
            {
                UserId = p.UserId,
                Username = p.User.Username,
                PostId = p.PostId,
                Text = p.Post.Text,
                Title = p.Post.Title,
                LikedByUser = true, //пользователь точно лайкнул этот пост
                // IsGuest = false
                // CreatedAt = p.CreatedAt
            })
            .ToListAsync(); //оборачиваем в список
            logger.LogWarning($"Посты которые лайкнул пользователь ({username}) получены");
            return likedPosts;
        }
        else
        {
            logger.LogWarning("Владелец профиля - {username} не найден", username);
            throw new Exception("Владелец профиля не найден");
        }
    }

    /*  public async Task<List<UserPostDto>> GetFavoritesPosts(string username, User user)
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
  }*/
}
