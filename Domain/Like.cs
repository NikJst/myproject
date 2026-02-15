namespace Testing3;
public class Like
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Guid PostId { get; private set; }

    public Like(Guid userId, Guid postId)
    {
        UserId = userId;
        PostId = postId;
    }

    // Конструктор для ORM (без параметров)
    public Like() { }
}