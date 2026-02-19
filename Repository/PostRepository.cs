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
    private readonly List<Post> _posts = new();
    public void Add(Post post)
    {
        Console.WriteLine($"Adding post with text: {post.Text} and user ID: {post.UserId}");
        _posts.Add(post);
        Console.WriteLine("Post added to repository");
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