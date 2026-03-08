using Testing3;

public interface IPostRepository
{
    void Add(Post post);
    void Remove(Guid postId);
    Post? GetPost(Guid postId);
    List<Post> GetAllPosts();
}
class PostRepository : IPostRepository
{
    private readonly ILogger<PostRepository> _logger;
    private readonly List<Post> _posts = [];
    public PostRepository(ILogger<PostRepository> logger)
    {
        _logger = logger;
    }
    public void Add(Post post)
    {
        _posts.Add(post);
        _logger.LogInformation($"Post created with ID: {post.GuidId}");

    }
    public void Remove(Guid postId)
    {
        _posts.RemoveAll(p => p.GuidId == postId);
    }
    public Post? GetPost(Guid postId)
    {
        return _posts.FirstOrDefault(p => p.GuidId == postId);
    }
    public List<Post> GetAllPosts()
    {
        return _posts.ToList();
    }
}