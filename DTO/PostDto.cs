
namespace Testing3.DTO;
public class ViewPostDto //для отображения всех постов
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public required string Text { get; set; }
    public string? Title { get; set; }
    public string? Username { get; set; }
    public bool IsOnline { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool MyLike { get; set; }
    public bool MyBookmark { get; set; }
    public int LikesCount { get; set; }
    public bool IsMyPost { get; set; }
}
public class PagedResponse<ViewPostDto>
{
    // public int UsersCount { get; set; }
    public IEnumerable<ViewPostDto> Items { get; set; } = [];
    public MetaData Meta { get; set; } = new MetaData();
}
public class MetaData
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}