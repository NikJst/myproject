namespace Testing3.DTO;
public class ViewPostsDto //DTO for displaying posts
{
    public Guid PostId { get; set; }  // Изменено с GuidId на PostId для соответствия фронтенду
    public string? Title { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool LikedByUser { get; set; }
    public int LikesCount { get; set; }  // Добавляем счетчик лайков
}

public class CreatePostDto //DTO for creating posts
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Title { get; set; }
    public bool LikedByUser { get; set; }
    public bool IsGuest { get; set; }
}

public class LikePostDto //DTO for toggling like on a post
{
    public Guid PostId { get; set; }
}
public class ViewUserCardDto // это карточки с пользователяи, пока рано
{
    public bool IsGuest { get; set; }
    public Guid GuidId { get; set; }
    public string Name { get; set; }
    public int PostCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOnline { get; set; } = false;
    public List<Post> Posts { get; private set; } = [];
}
public class ViewUsersListDto //для отображения списка пользователей с карточками и количеством пользователей в поиске
{
    public List<ViewUserCardDto> Users { get; set; } = [];
    public int UsersCount { get; set; }
}