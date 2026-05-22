namespace Testing3.DTO;
public class ViewPostsDto //для отображения всех постов
{
    public Guid PostId { get; set; }  // Изменено с GuidId на PostId для соответствия фронтенду
    public Guid UserId { get; set; }
    public required string Text { get; set; }
    public string? Title { get; set; }
    public bool LikedByUser { get; set; }
    public int LikesCount { get; set; }  // счетчик для каждой карточки
    public string? Username { get; set; } // Добавляем имя автора
    public bool IsOnline { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public bool? IsPublished { get; set; }
    public bool DeletedButton { get; set; }
}
public class LikePostDto //для переключения лайка на посте
{
    public Guid PostId { get; set; }
}

public class PagedResponse<T>
{
    // public int UsersCount { get; set; }
    public IEnumerable<ViewPostsDto> Items { get; set; } = [];
    public MetaData Meta { get; set; } = new MetaData();
}
public class MetaData
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}