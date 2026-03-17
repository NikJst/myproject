namespace Testing3;
public class Post
{
    public Post(string text, Guid userId, string? title = null)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        UserId = userId;
        Title = title;
    }

    public Guid GuidId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string? Title { get; private set; } = null;
    public bool IsPublished { get; set; } // true = видимо всем, false = черновик
    public Post() { }
}