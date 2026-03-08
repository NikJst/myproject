namespace Testing3;

public interface IPostService
{
    void CreatePost(string text, Guid userId);
    void DeletePost(Guid postId);
    Post? GetPost(Guid postId);
    List<Post> GetAllPosts();
    List<ViewPostDto> GetAllPostsForUser(Guid userId);
}

public class PostService : IPostService
{
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<PostService> _logger;
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository, ILikeRepository likeRepository, ILogger<PostService> logger) 
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _likeRepository = likeRepository ?? throw new ArgumentNullException(nameof(likeRepository));
        _logger = logger;
    }
    public void CreatePost(string text, Guid userId)
    {
        var post = new Post(text, userId);
        _logger.LogInformation($"Post created: {post.Text} with ID: {post.GuidId}");
        _postRepository.Add(post);
        _logger.LogInformation("Post added to repository");
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
            Text = p.Text,
            UserId = p.UserId,
            LikedByUser = _likeRepository.Exists(userId, p.GuidId) //доделать в будщем как один запрос к бд
        }).ToList();
    }
}