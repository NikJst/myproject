using System.ComponentModel.DataAnnotations;
namespace Testing3;
public class ViewPostDto //DTO for displaying posts with like status
{

    public string Text { get; set; } = string.Empty;
    [Required]
    public Guid UserId { get; set; }
    public bool LikedByUser { get; set; }
}

public class CreatePostDto //DTO for creating posts
{
    public string Text { get; set; } = string.Empty;
    public bool LikedByUser { get; set; } = false;
}

public class LikePostDto //DTO for toggling like on a post
{
    public Guid PostId { get; set; }
}
