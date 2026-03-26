namespace Testing3.DTO;
public class ViewPostsDto //DTO for displaying posts
{
    public Guid PostId { get; set; }  // Изменено с GuidId на PostId для соответствия фронтенду
    public string? Title { get; set; }
    public required string Text { get; set; }
    public required Guid UserId { get; set; }
    public bool LikedByUser { get; set; }
    public int LikesCount { get; set; }  // Добавляем счетчик лайков
    public required string Username { get; set; } // Добавляем имя автора
}

public class CreatePostDto //DTO for creating posts
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public required string Text { get; set; }
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
    public required Guid UserId { get; set; } // добавляем UserId
    public string? Username { get; set; } // добавляем Username
    public Guid GuidId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; } // добавляем Description
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


public class ProfileInfoDto //DTO для отображения профиля
{
    public required string Username { get; set; }
    public Guid UserId { get; set; } //---------для проверки прав доступа
    //     public string? AvatarUrl { get; set; }
    public string? Header { get; set; }
    //     public string? Location { get; set; }
    public string? Description { get; set; }

    // public bool IsGuest { get; set; } 
    public int PostCount { get; set; } // добавляем счетчик постов
    public int LikesCount { get; set; } // добавляем счетчик лайков
                                        //     // public Guid GuidId { get; set; }
                                        //     // public DateTime CreatedAt { get; set; }
                                        //     public bool IsOnline { get; set; } = false;
    public List<Post> Posts { get; set; } = [];
    //     public List<Like> Likes { get; set; } = []; // пока не уверен
}

/// DTO для постов пользователя

public class UserPostDto
{
    //кнопка редактирования поста
    public required string Username { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public required string Text { get; set; } = string.Empty;// возможный черновик
    public string? Title { get; set; }
    public bool LikedByUser { get; set; }
    public bool IsGuest { get; set; }
    // public DateTime CreatedAt { get; set; }
}
public class UserPostsDto
{
    public List<UserPostDto> Posts { get; set; } = [];
}
