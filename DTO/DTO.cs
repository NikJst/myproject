namespace Testing3;
public class LikedPostDto //DTO for displaying posts with like status
{
    public string Text { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool LikedByUser { get; set; }
}

public class BasePostDto //DTO for creating posts
{
    public string Text { get; set; } = string.Empty;
}

public class LikePostDto //DTO for toggling like on a post
{
    public Guid PostId { get; set; }
}
