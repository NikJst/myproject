namespace Testing3;
using Testing3.DTO;

public interface IPostService
{
    Task<Post> CreatePostAsync(string text, Guid userId, string? title = null); // дописать dto на проверку гостя для черновика
    void DeletePost(Guid postId);
    Post? GetPost(Guid postId);
    List<Post> GetAllPosts();
    List<ViewPostDto> GetAllPostsForUser(Guid userId);
}
public class PostService : IPostService
{

    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<PostService> _logger;

    private readonly ApplicationDbContext _context;
    public PostService(ApplicationDbContext dbContext, ILogger<PostService> logger)
    {
        _context = dbContext;
        _logger = logger;
    }
    public async Task<Post> CreatePostAsync(string text, Guid userId, string? title = null )
    {
        await _context.Posts.AddAsync(new CreatePostDto

        {
            Text = text,
            Title = title,
        });
        await _context.SaveChangesAsync();
        return _context.Posts.Last();
    }

    public void DeletePost(Guid postId)
    {
        _logger.LogInformation($"Deleting post with ID: {postId}");
        _postRepository.Remove(postId);
    }

    public Post? GetPost(Guid postId)
    {
        _logger.LogInformation($"Getting post with ID: {postId}");
        return _postRepository.GetPost(postId);
    }

    public List<Post> GetAllPosts()
    {
        _logger.LogInformation("Getting all posts");
        return _postRepository.GetAllPosts();
    }

    public List<ViewPostDto> GetAllPostsForUser(Guid userId)
    {
        var posts = _postRepository.GetAllPosts();

        return posts.Select(p => new ViewPostDto
        {
            GuidId = p.GuidId,
            Text = p.Text,
            Title = p.Title,
            UserId = p.UserId,
            LikedByUser = _likeRepository.Exists(userId, p.GuidId) //доделать в будщем как один запрос к бд
        }).ToList();
    }
}