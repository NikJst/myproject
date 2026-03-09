namespace Testing3.DTO;
public class ViewPostDto //DTO for displaying posts with like status
{
    public Guid GuidId { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool LikedByUser { get; set; }
}

public class CreatePostDto //DTO for creating posts
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool LikedByUser { get; set; }
}

public class LikePostDto //DTO for toggling like on a post
{
    public Guid PostId { get; set; }
}
