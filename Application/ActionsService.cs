
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Testing3.Application;

public interface IActionService
{
    Task<ActionsDto> GetMyAction([FromBody] string[] ids, User me);

    // Task<IEnumerable<string>> GetOnlineUsersAsync();
}

public class ActionsDto
{
    public Dictionary<string, bool> FlagsMyLikes { get; set; }
    // public bool Like { get; set; }
    // public bool Bookmark { get; set; }
    // public bool Repost { get; set; }
}
public class ActionService : IActionService
{
    private readonly IUserService userService;
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger logger;
    public ActionService(IUserService userService, ILogger<ActionService> logger, ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.logger = logger;
        this.userService = userService;
    }
    // public Expression<Func<ApplicationDbContext, IQueryable<Like>>> CreateExpress(string postId, string meId) //для каждой таблицы свое выражение 
    // {
    //     // Выражение, указывающее, что мы берем всю таблицу Like
    // }
    public async Task<ActionsDto> GetMyAction([FromBody] string[] ids, User me)
    {

        var idList = ids;
        // .Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();

        Dictionary<string, bool> flagsMyLikes = new Dictionary<string, bool>();

        foreach (var item in idList)
        {
            var x = Guid.TryParse(item, out Guid postid);
            var qq = await dbContext.Likes.Where(l => l.UserId == me.Id && l.PostId == postid).AnyAsync();
            flagsMyLikes.Add(item, qq);
        }
        return new ActionsDto
        {
            FlagsMyLikes = flagsMyLikes
        };

    }
}




// public Expression<Func<Like, bool>> CreateExpressionLike(string postids, string meId)
// {
//     // Expression<Func<ApplicationDbContext, IQueryable<Like>>> tableExpression = db => db.Likes;

//     var like = Expression.Parameter(typeof(Like), "l");
//     var uId = Expression.Property(like, nameof(Like.UserId));
//     var pId = Expression.Property(like, nameof(Like.PostId));  //передаем как конст

//     var constPost = Expression.Constant(postids);
//     var constUser = Expression.Constant(meId);
//     BinaryExpression equalUserId = Expression.Equal(uId, constUser);//мы поставили лайк этому посту 
//     BinaryExpression equalpostId = Expression.Equal(pId, constPost);//мы поставили лайк этому посту 

//     var lambda = Expression.Lambda<Func<Like, bool>>(equalUserId, equalpostId, like);
//     System.Console.WriteLine(lambda);
//     return lambda;

// }
// public Expression<Func<Bookmark, bool>> CreateExpressionBookmark()
// {
//     // Expression<Func<ApplicationDbContext, IQueryable<Bookmark>>> tableExpression = db => db.Bookmarks;

//     var like = Expression.Parameter(typeof(Bookmark), "b");
//     var uId = Expression.Property(like, nameof(Bookmark.UserId));
//     var pId = Expression.Property(like, nameof(Bookmark.PostId));  //передаем как конст
//     var constPost = Expression.Constant(pId);
//     var constUser = Expression.Constant(uId);
//     BinaryExpression equal = Expression.Equal(constPost, constUser);//мы поставили лайк этому посту 
//     var lambda = Expression.Lambda<Func<Bookmark, bool>>(equal, like);
//     System.Console.WriteLine(lambda);
//     return lambda;

// }
