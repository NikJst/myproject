namespace Testing3;
public class Post
{
    public Post(string text, Guid userId)
    {
        Text = text ?? throw new ArgumentNullException("text is required");
        UserId = userId;
    }
    public Guid GuidId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Text { get; private set; } = string.Empty;


    // Конструктор для ORM (без параметров)
    public Post() { }
}