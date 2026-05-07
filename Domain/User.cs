using System.Runtime.CompilerServices;

namespace Testing3;
public class User
{
    // для отображения профиля
    public string? Username { get; set; }
    // public string? AvatarUrl { get; set; }
    public string? Header { get; set; }
    // public string? Location { get; set; }
    public string? Description { get; set; }
    // остальное -->
    public bool IsGuest { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int PostCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOnline { get; set; } = false;
    public List<Post> Posts { get; private set; } = []; // навигация к Post 
    public List<Like> Likes { get; private set; } = []; // навигация к Like
                                                        // Parameterless constructor for EF
    public User(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsGuest = true;
        // CreatedAt будет установлен явно при создании
    }

}
