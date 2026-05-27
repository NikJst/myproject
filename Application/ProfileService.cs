using Testing3.DTO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
namespace Testing3.Application;

public interface IProfileService
{
    Task<ProfileEditDto> PatchUserInfo(ProfileEditDto profileDto, User user, string username);
    Task<ProfileInfoDto> GetTargetProfile(string username, User user);
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
    public async Task<User> GetTargetUser(string username)
    {
        logger.LogWarning($"GetTargetUser called with username: {username}");
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        System.Console.WriteLine("получили имя {}");
        if (user == null)
        {
            logger.LogWarning("Пользователь не найден");
        }
        return user;
    }
    public async Task<ProfileEditDto> PatchUserInfo(ProfileEditDto profileDto, User me, string username)
    {
        var targetus = await GetTargetUser(username);


        // var me = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == me.Id);

        if (me.Id.Equals(targetus.Id))
        {
            me.Username = profileDto.Username;
            me.Age = profileDto.Age;
            me.Gender = profileDto.Gender;
            me.City = profileDto.City;
            await dbContext.SaveChangesAsync();

            logger.LogWarning($"Profile обновлен в бд");
            return new ProfileEditDto
            {
                Username = me.Username,
                Header = me.Header,
                Description = me.Description,
                Age = me.Age,
                Gender = me.Gender,
                City = me.City,
            };
        }
        throw new InvalidOperationException("запрещено редактирвать чужой профиль");
    }
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
            return new ProfileInfoDto
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
        }
        else
        {
            return new ProfileInfoDto
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
        }
    }
}

public static class PostDtoBuilder
{
    public static Expression<Func<Post, ViewPostDto>> CreateExpression(Guid userId, ILogger? logger = null)
    {
        logger?.LogWarning("CreateExpression called for userId: {UserId}", userId);
        return post => new ViewPostDto
        {
            UserId = post.UserId,
            Username = post.User.Username,
            PostId = post.Id,
            Text = post.Text,
            Title = post.Title,
            // MyLike = post.Likes.Any(l => l.PostId == post.Id && l.UserId == userId),
            // LikesCount = post.Likes.Count(l => l.PostId == post.Id),
            // MyBookmark = post.Bookmarks.Any(b => b.PostId == post.Id && b.UserId == userId),
            IsPublished = post.IsPublished,
            CreatedAt = post.CreatedAt
        };
    }
}

public interface ITest
{
    Task<List<ViewPostDto>> TestMethod(User me, char i, string username);
}
public class Test : ITest
{
    public Test(ApplicationDbContext dbContext, ILogger<Test> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<Test> logger;

    public async Task<List<ViewPostDto>> TestMethod(User me, char i, string username)
    {
        var targetuser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        switch (i)
        {
            case '1':
                var query = dbContext.Posts
                .Where(p => p.UserId == targetuser.Id);

                var result = await query.Select(PostDtoBuilder.CreateExpression(me.Id, logger)).ToListAsync();
                logger.LogWarning("Case 1 completed, returned {Count} posts", result.Count);

                string sqlText = query.ToQueryString();
                logger.LogWarning("SQL Query: {SqlText}", sqlText);

                return result;
            case '2':
                var query1 = dbContext.Posts
                .Where(p => p.Bookmarks
                .Any(b => b.UserId == targetuser.Id));

                var result1 = await query1.Select(PostDtoBuilder.CreateExpression(me.Id, logger)).ToListAsync();
                logger.LogWarning("Case 2 completed, returned {Count} posts", result1.Count);

                string sqlText1 = query1.ToQueryString();
                logger.LogWarning("SQL Query: {SqlText}", sqlText1);
                return result1;
            case '3':
                var query2 = dbContext.Posts
                .Where(l => l.Likes
                .Any(b => b.UserId == targetuser.Id));

                var result2 = await query2.Select(PostDtoBuilder.CreateExpression(me.Id, logger)).ToListAsync();
                logger.LogWarning("Case 3 completed, returned {Count} posts", result2.Count);

                string sqlText2 = query2.ToQueryString();
                // logger.LogWarning("SQL Query: {SqlText}", sqlText2);
                return result2;
            default:
                logger.LogWarning("Unknown option: {Option}", i);
                return new List<ViewPostDto>();
        }
    }
}


//--------------->  получение собственных постов пользователя
// public async Task<List<ViewPostDto>> GetTargetUserPosts(string username, User user)
// {
//     var someUser = await GetTargetUser(username);

//     var query = dbContext.Posts
//     .Where(p => p.UserId == someUser.Id) //среди всех постов находим таргетный
//     .Select(p => new ViewPostDto
//     {
//         UserId = p.UserId,
//         Username = someUser.Username,
//         PostId = p.Id,
//         Text = p.Text,
//         Title = p.Title,
//         IsMyPost = true,
//         MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id), //сравнение с конкретным postId
//         LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id), //сравнение с конкретным postId
//         MyBookmark = dbContext.Bookmarks.Any(b => b.UserId == user.Id && p.Id == b.PostId), //сравнение с конкретным postId
//     });
//     var response = await query
//     .Skip(0).Take(10).ToListAsync(); //оборачиваем в список
//     logger.LogWarning($"Посты пользователя {username} получены");
//     return response;
// }

//    public async Task<List<ViewPostDto>> GetLikesPosts(string username, User user)
//    {
//        var someUser = await GetTargetUser(username);

//        var likedPosts = await dbContext.Posts
//        .Where(p => p.IsPublished)
//        .Where(p => dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == someUser.Id))
//            .Select(p => new ViewPostDto
//            {
//                UserId = p.UserId,
//                Username = p.User.Username,
//                PostId = p.Id,
//                Text = p.Text,
//                Title = p.Title,
//                IsMyPost = p.UserId == user.Id,
//                // MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id),
//                MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id), //сравнение с конкретным postId
//                LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id), //сравнение с конкретным postId
//                MyBookmark = dbContext.Bookmarks.Any(b => b.UserId == user.Id && b.PostId == p.Id && p.UserId == user.Id), //сравнение с конкретным postId
//                //     IsPublished = p.IsPublished,
//                //     CreatedAt = p.CreatedAt
//            })
//            .ToListAsync(); //оборачиваем в список

//        return likedPosts;
//    }

//    public async Task<List<ViewPostDto>> GetDraftsPosts(string username, User user)
//    {
//        var someUser = await GetTargetUser(username);
//        // Проверяем, что это свой профиль (черновики может видеть только автор)
//        if (user.Id != someUser.Id)
//        {
//            throw new Exception("Доступ к черновикам запрещен");
//        }

//        var draftPosts = await dbContext.Posts
//        .Where(p => !p.IsPublished) //неопубликованные посты пользователя
//        .Select(p => new ViewPostDto
//        {
//            UserId = p.UserId,
//            Username = someUser.Username,
//            PostId = p.Id,
//            Text = p.Text,
//            Title = p.Title,
//            MyLike = dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == user.Id),
//            LikesCount = dbContext.Likes.Count(l => l.PostId == p.Id),
//            IsPublished = false,
//            MyBookmark = dbContext.Bookmarks.Any(b => b.PostId == p.Id && b.UserId == user.Id),
//            CreatedAt = p.CreatedAt
//        })
//        .ToListAsync(); //оборачиваем в список
//        logger.LogWarning($"Черновики пользователя ({username}) получены");
//        return draftPosts;
//    }
//    public async Task<List<ViewPostDto>> GetBookmarksPosts(string username, User user)
//    {

//        var someUser = await GetTargetUser(username);

//        logger.LogWarning($"Target user: {someUser?.Username}");
//        // Проверяем, что это свой профиль (закладки может видеть только автор)
//        if (user.Id != someUser.Id)
//        {
//            throw new Exception("Доступ к чужим закладкам запрещен");
//        }

//        var bookmarkedPosts = await dbContext.Bookmarks
//        .Where(b => b.UserId == someUser.Id) //закладки пользователя
//        .Select(b => new ViewPostDto
//        {
//            UserId = b.Post.User.Id,
//            Username = b.Post.User.Username,
//            PostId = b.Post.Id,
//            Text = b.Post.Text,
//            Title = b.Post.Title,
//            MyLike = dbContext.Likes.Any(l => l.PostId == b.Post.Id && l.UserId == user.Id),
//            LikesCount = dbContext.Likes.Count(l => l.PostId == b.Post.Id),
//            MyBookmark = true,
//            IsPublished = b.Post.IsPublished,
//            CreatedAt = b.Post.CreatedAt
//        })
//        .ToListAsync(); //оборачиваем в список
//        logger.LogWarning($"Закладки пользователя ({username}) получены");
//      return bookmarkedPosts;
//    }
// }

