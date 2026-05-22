using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Testing3;
// [Table("users", Schema = "public")]
public class User
{

    public string Name { get; set; }
    [StringLength(10)]
    public string? Username { get; set; }
    // public string? AvatarUrl { get; set; }
    [StringLength(200)]
    public string? Header { get; set; }
    // public string? Location { get; set; }
    public string? Description { get; set; }
    // остальное -->
    [Range(1, 60)]
    public int Age { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Hobby { get; set; }
    public string? Interests { get; set; }
    public bool IsGuest { get; set; }
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public int PostCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Post> Posts { get; private set; } = []; // навигация к Post 
    public List<Like> Likes { get; private set; } = []; // навигация к Like
                                                        // Parameterless constructor for EF
    public User(string? name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsGuest = true;
        // CreatedAt будет установлен явно при создании
    }

}
