using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Testing3;
public class Like
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid? UserId { get; private set; }
    public Guid? PostId { get; private set; }

    public User? User { get; set; }  // навигация к User
    public Post? Post { get; set; }  // навигация к Post

    public Like(Guid? userId, Guid? postId)
    {
        UserId = userId;
        PostId = postId;
    }

    // Конструктор для ORM (без параметров)
    public Like() { }
}