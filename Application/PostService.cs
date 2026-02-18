namespace Testing3;
public class PostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public void CreatePost(string text, Guid userId)
    {
        var post = new Post(text, userId);
        _postRepository.Add(post);
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