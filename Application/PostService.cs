namespace Testing3;
using Testing3.DTO;
using Microsoft.EntityFrameworkCore;

public interface IPostService
{
    Task<Post> CreatePostAsync(CreatePostDto request); // дописать dto на проверку гостя для черновика
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
    public async Task<Post> CreatePostAsync(CreatePostDto request)
    {
        var post = new Post(request.Text, request.UserId, request.Title);
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

        .Include(p => _dbcontext.Likes) // включить связанные объекты
        // .Where(p => _dbcontext.Likes.Any(l => l.UserId == userId && l.PostId == p.PostId)) оставить эту строку для того чтобы показывать пользователю посты которые он лайкнул
        .Select(p => new ViewPostsDto // мы выбираем что отдавать клиенту
        {
            GuidId = p.PostId,
            Text = p.Text,
            Title = p.Title,
            LikedByUser = _dbcontext.Likes.Any(l => l.UserId == userId && l.PostId == p.PostId)
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