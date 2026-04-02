using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace Testing3;
public class Post
{
    public Post(string text, Guid userId, string? title = null)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        UserId = userId;
        Title = title;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string? Title { get; private set; } = null;
    public DateTime CreatedAt { get; private set; }
    public bool IsPublished { get; set; } // true = видимо всем, false = черновик
    // public NpgsqlTsVector? SearchVector { get; set; }
    public List<Like> Likes { get; private set; } = [];

    // Навигационное свойство для entity framework
    public User User { get; set; } = null!;
}