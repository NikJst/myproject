using Testing3.DTO;
using Microsoft.EntityFrameworkCore;
namespace Testing3.Application;

public interface IProfileService
{
    Task<ProfileEditDto> PatchInfo(ProfileEditDto profileDto, User user);
    Task<ProfileInfoDto> GetTargetProfile(string username, User user);
    Task<PagedResponse<ViewPostsDto>> GetTargetUserPosts(string username, User user);
    // Task<ProfileDto> GetUserProfileContent(Guid userId);
    Task<List<ViewPostsDto>> GetLikesPosts(string username, User user);
    Task<List<ViewPostsDto>> GetDraftsPosts(string username, User user);
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
    public async Task<ProfileEditDto> PatchInfo(ProfileEditDto profileDto, User user)
    {
        logger.LogWarning("PatchInfo called");
        var userdata = await dbContext.Users.FindAsync(user.Id);
        if (user.Id == userdata.Id)
        {
            logger.LogWarning($"Profile обновляется для пользователя {user.Id}");

            userdata.Age = profileDto.Age;
            userdata.Gender = profileDto.Gender;
            userdata.City = profileDto.City;
            userdata.Street = profileDto.Street;
            userdata.Hobby = profileDto.Hobby;
            userdata.Interests = profileDto.Interests;
            await dbContext.SaveChangesAsync();

            logger.LogWarning($"Profile обновлен в бд");
            var profileInfo = new ProfileEditDto
            {
                Header = userdata.Header,
                Description = userdata.Description,
                Age = userdata.Age,
                Gender = userdata.Gender,
                City = userdata.City,
                Street = userdata.Street,
                Hobby = userdata.Hobby,
                Interests = userdata.Interests
            };
            return profileInfo;
        }
        throw new Exception("Ошибка обновления профиля");
    }


    //-------->
    public async Task<ProfileInfoDto> GetTargetProfile(string username, User user)
    // таким образом получается что у нас один объект ProfileDto для всех операций, но с разным содержимым
    {
        var targetUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username);
        // Считаем посты пользователя
        var postCountMe = await dbContext.Posts
                       .Where(p => p.UserId == user.Id)
                       .CountAsync();

        // Считаем созданные лайки пользователя
        var likesCountMe = await dbContext.Likes
        .Where(l => l.UserId == user.Id)
        .CountAsync();

        if (targetUser != null)
        {

            if (user.Id == targetUser.Id) // если это наш профиль
            {


                var userProfileInfo = new ProfileInfoDto
                {
                    UserId = targetUser.Id,// тебе нужен
                    Username = targetUser.Username,
                    Header = targetUser.Header,
                    Description = targetUser.Description,
                    IsOnline = false,
                    Age = targetUser.Age,
                    Gender = targetUser.Gender,
                    City = targetUser.City,
                    Street = targetUser.Street,
                    Hobby = targetUser.Hobby,
                    Interests = targetUser.Interests,

                    PostCount = postCountMe,
                    LikesCount = likesCountMe
                };
                logger.LogWarning($"Profile получен для нашего профиля");
                return userProfileInfo;
            }
            else
            {
                var alienProfileInfo = new ProfileInfoDto
                {
                    // тебе не нужны эти данные для чужого профиля
                    UserId = targetUser.Id,
                    Username = targetUser.Username,
                    Header = targetUser.Header,
                    Description = targetUser.Description,
                    IsOnline = false,
                    Age = targetUser.Age,
                    Gender = targetUser.Gender,
                    City = targetUser.City,
                    Street = targetUser.Street,
                    Hobby = targetUser.Hobby,
                    Interests = targetUser.Interests,

                    PostCount = postCountMe,
                    LikesCount = likesCountMe,
                };
                return alienProfileInfo;
            }
        }

        else
        {
            logger.LogWarning($"Владелец профиля - {username} не найден");
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
            .Where(p => p.UserId == targetUser.Id) //среди всех постов находим таргетный
            .Select(p => new ViewPostsDto
            {
                UserId = p.UserId,
                Username = targetUser.Username,
                PostId = p.Id,
                Text = p.Text,
                Title = p.Title,
                LikedByUser = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id),
                LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id)
            });
            var response = await query.Skip(0).Take(10).ToListAsync(); //оборачиваем в список
            logger.LogWarning($"Посты пользователя {username} получены");
            return new PagedResponse<ViewPostsDto> { Items = response, Meta = new MetaData() }; //в коллекцию передаем список постов и в будщем еще что то
        }
        else
        {
            logger.LogWarning($"Владелец профиля - {username} не найден", username);
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
            .Where(l => l.UserId == usernameUser.Id) //лайки пользователя
            .Select(l => new ViewPostsDto
            {
                UserId = l.Id,
                Username = l.User.Username,
                PostId = l.Id,
                Text = l.Post.Text,
                Title = l.Post.Title,
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
            logger.LogWarning($"Владелец профиля - {username} не найден", username);
            throw new Exception("Владелец профиля не найден");
        }
    }

    public async Task<List<ViewPostsDto>> GetDraftsPosts(string username, User user)
    {
        var usernameUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username);

        if (usernameUser != null)
        {
            // Проверяем, что это свой профиль (черновики может видеть только автор)
            if (user.Id != usernameUser.Id)
            {
                throw new Exception("Доступ к черновикам запрещен");
            }

            var draftPosts = await dbContext.Posts
            .Where(p => p.UserId == usernameUser.Id && !p.IsPublished) //неопубликованные посты пользователя
            .Select(p => new ViewPostsDto
            {
                UserId = p.UserId,
                Username = usernameUser.Username,
                PostId = p.Id,
                Text = p.Text,
                Title = p.Title,
                LikedByUser = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id),
                LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id),
                IsPublished = p.IsPublished,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(); //оборачиваем в список
            logger.LogWarning($"Черновики пользователя ({username}) получены");
            return draftPosts;
        }
        else
        {
            logger.LogWarning($"Владелец профиля - {username} не найден", username);
            throw new Exception("Владелец профиля не найден");
        }
    }
}
