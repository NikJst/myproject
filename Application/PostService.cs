namespace Testing3;
using Testing3.DTO;
using Microsoft.EntityFrameworkCore;

public interface IPostService
{
    Task<Post> CreatePostAsync(string text, string? title = null, Guid? userId = null); // дописать dto на проверку гостя для черновика
    void DeletePost(Guid postId);
    Post? GetPost(Guid postId);
    List<Post> GetAllPosts();
    Task<List<ViewPostsDto>> GetAllPostsForUserAsync(Guid userId);
}
public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly ApplicationDbContext _dbcontext;
    public PostService(ApplicationDbContext dbContext, ILogger<PostService> logger)
    {
        _dbcontext = dbContext;
        _logger = logger;
    }
    public async Task<Post> CreatePostAsync(string text, string? title = null, Guid? userId = null)
    {
        var post = new Post(text, userId ?? Guid.Empty, title);
        await _dbcontext.Posts.AddAsync(post);
        await _dbcontext.SaveChangesAsync();
        return post;
    }

    public Post? GetPost(Guid postId)
    {
        _logger.LogInformation($"Getting post with ID: {postId}");
        return _dbcontext.Posts.FirstOrDefault(p => p.PostId == postId);
    }

    public List<Post> GetAllPosts()
    {
        _logger.LogInformation("Getting all posts");
        return _dbcontext.Posts.ToList();
    }

    public Task<List<ViewPostsDto>> GetAllPostsForUserAsync(Guid userId)
    {

        var posts = _dbcontext.Posts
        .Include(p => p.Likes) // включить связанные объекты
        .Select(p => new ViewPostsDto // мы выбираем что отдавать клиенту
        {
            UserId = p.UserId,
            PostId = p.PostId,  // Изменено с GuidId на PostId
            Text = p.Text,
            Title = p.Title,
            LikedByUser = p.Likes.Any(l => l.UserId == userId), //измененный и правильный вариант
            LikesCount = p.Likes.Count  // Добавляем подсчет лайков

            //то что было LikedByUser = _dbcontext.Likes.Any(l => l.UserId == userId && l.PostId == p.PostId)

            // ===> то что стало | теперь мы обращаемся к посту, заходим в его в лайки, потом находим соответствие если среди этих лайков тот, который поставил определённый пользователь(userid == userid) 
            // Мы больше не лезем в _dbcontext.Likes вручную!
            // Мы спрашиваем СУБЪЕКТИВНО у поста: "Есть ли среди ТВОИХ лайков мой?
        })
        .ToListAsync();
        return posts;

    }
    public void DeletePost(Guid postId)
    {
        _logger.LogInformation($"Deleting post with ID: {postId}");
        var post = _dbcontext.Posts.FirstOrDefault(p => p.PostId == postId);
        if (post != null)
        {
            _dbcontext.Posts.Remove(post);
            _dbcontext.SaveChanges();
        }
    }
}