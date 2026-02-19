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
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository) =>
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));

    public void CreatePost(string text, Guid userId)
    {
        Console.WriteLine($"Creating post with text: {text} and user ID: {userId}");
        var post = new Post(text, userId);
        Console.WriteLine($"Post created: {post.Text} with ID: {post.GuidId}");
        _postRepository.Add(post);
        Console.WriteLine("Post added to repository");
    }

    public void DeletePost(Guid postId)
    {
        _postRepository.Remove(postId);
    }

    public Post? GetPost(Guid postId)
    {
        return _postRepository.GetPost(postId);
    }

    public List<Post> GetAllPosts()
    {
        return _postRepository.GetAllPosts();
    }
}