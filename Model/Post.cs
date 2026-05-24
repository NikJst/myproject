using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace Testing3;
public class Post
{
    public Post(string text, Guid userId, bool isPublished, string? title = null)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        UserId = userId;
        Title = title;
        IsPublished = isPublished;
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
    [Required]
    public bool IsPublished { get; set; } // true = видимо всем, false = черновик

    public List<Like> Likes { get; private set; } = [];
    public List<Bookmark> Bookmarks { get; private set; } = [];
    public User User { get; set; } = null!; // с virtual проблема N+1
}