namespace Testing3;

public interface IPostService
{
    void CreatePost(string text, Guid userId);
    void DeletePost(Guid postId);
    Post? GetPost(Guid postId);
    List<Post> GetAllPosts();
}

public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository, ILogger<PostService> logger) =>
        (_postRepository, _logger) = (postRepository ?? throw new ArgumentNullException(nameof(postRepository)), logger);

    public void CreatePost(string text, Guid userId)
    {
        _logger.LogInformation("Creating post with text: {Text} and user ID: {UserId}", text, userId);
        var post = new Post(text, userId);
        _logger.LogInformation("Post created: {Text} with ID: {GuidId}", post.Text, post.GuidId);
        _postRepository.Add(post);
        _logger.LogInformation("Post added to repository");
    }

    public void DeletePost(Guid postId)
    {
        _logger.LogInformation("Deleting post with ID: {PostId}", postId);
        _postRepository.Remove(postId);
    }

    public Post? GetPost(Guid postId)
    {
        _logger.LogInformation("Getting post with ID: {PostId}", postId);
        return _postRepository.GetPost(postId);
    }

    public List<Post> GetAllPosts()
    {
        _logger.LogInformation("Getting all posts");
        return _postRepository.GetAllPosts();
    }
}