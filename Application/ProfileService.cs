using Testing3.DTO;
using Microsoft.EntityFrameworkCore;
namespace Testing3.Application;

public interface IProfileService
{
    Task<ProfileEditDto> PatchUserInfo(ProfileEditDto profileDto, User user);
    Task<ProfileInfoDto> GetTargetProfile(string username, User user);
    Task<List<ViewPostDto>> GetTargetUserPosts(string username, User user);
    // Task<ProfileDto> GetUserProfileContent(Guid userId);
    Task<List<ViewPostDto>> GetLikesPosts(string username, User user);

    Task<List<ViewPostDto>> GetBookmarksPosts(string username, User user);

    Task<List<ViewPostDto>> GetDraftsPosts(string username, User user);
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
    public async Task<User> GetTargetUser(string username)
    {
        logger.LogWarning($"GetTargetUser called with username: {username}");
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            logger.LogWarning("Пользователь не найден");
        }
        await Task.CompletedTask;
        return user;
    }
    public async Task<ProfileEditDto> PatchUserInfo(ProfileEditDto profileDto, User user)
    {
        logger.LogWarning("PatchUserInfo called");
        var userdata = await dbContext.Users.FindAsync(user.Id);
        if (userdata != null && user.Id == userdata.Id)
        {
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
        var someUser = await GetTargetUser(username);
        // Считаем посты пользователя
        var postCountProfile = await dbContext.Posts
        .Where(p => p.UserId == someUser.Id)
        .CountAsync();
        // Считаем созданные лайки пользователя
        var likesCountProfile = await dbContext.Likes
        .Where(l => l.UserId == someUser.Id)
        .CountAsync();
        var bookmarksCountProfile = await dbContext.Bookmarks
        .Where(b => b.UserId == someUser.Id)
        .CountAsync();


        if (user.Id == someUser.Id)// если это наш профиль
        {
            var userProfileInfo = new ProfileInfoDto
            {
                UserId = someUser.Id,// тебе нужен
                Username = username,
                Header = someUser.Header,
                Description = someUser.Description,
                Age = someUser.Age,
                Gender = someUser.Gender,
                City = someUser.City,
                Street = someUser.Street,
                Hobby = someUser.Hobby,
                Interests = someUser.Interests,

                PostCount = postCountProfile,
                LikesCount = likesCountProfile,
                BookmarksCount = bookmarksCountProfile
            };
            logger.LogWarning($"Profile получен для нашего профиля");
            return userProfileInfo;
        }
        else
        {
            var alienProfileInfo = new ProfileInfoDto
            {
                // тебе не нужны эти данные для чужого профиля
                UserId = someUser.Id,
                Username = username,
                Header = someUser.Header,
                Description = someUser.Description,
                Age = someUser.Age,
                Gender = someUser.Gender,
                City = someUser.City,
                Street = someUser.Street,
                Hobby = someUser.Hobby,
                Interests = someUser.Interests,

                PostCount = postCountProfile,
                LikesCount = likesCountProfile,
                BookmarksCount = bookmarksCountProfile
            };
            return alienProfileInfo;
        }
    }


    //--------------->  получение собственных постов пользователя
    public async Task<List<ViewPostDto>> GetTargetUserPosts(string username, User user)
    {
        var someUser = await GetTargetUser(username);

        var query = dbContext.Posts
        .Where(p => p.UserId == someUser.Id) //среди всех постов находим таргетный
        .Select(p => new ViewPostDto
        {
            UserId = p.UserId,
            Username = someUser.Username,
            PostId = p.Id,
            Text = p.Text,
            Title = p.Title,
            IsMyPost = true,
            MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id), //сравнение с конкретным postId
            LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id), //сравнение с конкретным postId
            MyBookmark = dbContext.Bookmarks.Any(b => b.UserId == user.Id && p.Id == b.PostId), //сравнение с конкретным postId
        });
        var response = await query
        .Skip(0).Take(10).ToListAsync(); //оборачиваем в список
        logger.LogWarning($"Посты пользователя {username} получены");
        return response;
    }

    public async Task<List<ViewPostDto>> GetLikesPosts(string username, User user)
    {
        var someUser = await GetTargetUser(username);

        var likedPosts = await dbContext.Posts
        .Where(p => p.IsPublished)
        .Where(p => dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == someUser.Id))
            .Select(p => new ViewPostDto
            {
                UserId = p.UserId,
                Username = p.User.Username,
                PostId = p.Id,
                Text = p.Text,
                Title = p.Title,
                IsMyPost = p.UserId == user.Id,
                // MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id),
                MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id), //сравнение с конкретным postId
                LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id), //сравнение с конкретным postId
                MyBookmark = dbContext.Bookmarks.Any(b => b.UserId == user.Id && b.PostId == p.Id && p.UserId == user.Id), //сравнение с конкретным postId
                //     IsPublished = p.IsPublished,
                //     CreatedAt = p.CreatedAt
            })
            .ToListAsync(); //оборачиваем в список

        return likedPosts;
    }

    public async Task<List<ViewPostDto>> GetDraftsPosts(string username, User user)
    {
        var someUser = await GetTargetUser(username);
        // Проверяем, что это свой профиль (черновики может видеть только автор)
        if (user.Id != someUser.Id)
        {
            throw new Exception("Доступ к черновикам запрещен");
        }

        var draftPosts = await dbContext.Posts
        .Where(p => !p.IsPublished) //неопубликованные посты пользователя
        .Select(p => new ViewPostDto
        {
            UserId = p.UserId,
            Username = someUser.Username,
            PostId = p.Id,
            Text = p.Text,
            Title = p.Title,
            MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id),
            LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id),
            IsPublished = false,
            MyBookmark = dbContext.Bookmarks.Any(b => b.PostId == p.Id && b.UserId == user.Id),
            CreatedAt = p.CreatedAt
        })
        .ToListAsync(); //оборачиваем в список
        logger.LogWarning($"Черновики пользователя ({username}) получены");
        return draftPosts;
    }
    public async Task<List<ViewPostDto>> GetBookmarksPosts(string username, User user)
    {

        var someUser = await GetTargetUser(username);

        logger.LogWarning($"Target user: {someUser?.Username}");
        // Проверяем, что это свой профиль (закладки может видеть только автор)
        if (user.Id != someUser.Id)
        {
            throw new Exception("Доступ к чужим закладкам запрещен");
        }

        var bookmarkedPosts = await dbContext.Bookmarks
        .Where(b => b.UserId == someUser.Id) //закладки пользователя
        .Select(b => new ViewPostDto
        {
            UserId = b.Post.User.Id,
            Username = b.Post.User.Username,
            PostId = b.Post.Id,
            Text = b.Post.Text,
            Title = b.Post.Title,
            MyLike = dbContext.Likes.Any(l => l.PostId == b.Post.Id && l.UserId == user.Id),
            LikesCount = dbContext.Likes.Count(l => l.PostId == b.Post.Id),
            MyBookmark = true,
            IsPublished = b.Post.IsPublished,
            CreatedAt = b.Post.CreatedAt
        })
        .ToListAsync(); //оборачиваем в список
        logger.LogWarning($"Закладки пользователя ({username}) получены");
        return bookmarkedPosts;
    }
}
