namespace Testing3;
public class User
{
    public bool IsGuest { get; set; } = true;
    public Guid GuidId { get; set; }
    public string Name { get; set; }
    public int PostCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOnline { get; set; } = true;
    public List<Post> Posts { get; private set; } = [];
    public User(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsGuest = true;
        CreatedAt = DateTime.Now;
    }

}
