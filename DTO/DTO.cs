namespace Testing3.DTO;
public class ViewPostsDto //для отображения всех постов
{
    public Guid PostId { get; set; }  // Изменено с GuidId на PostId для соответствия фронтенду
    public Guid UserId { get; set; }
    public required string Text { get; set; }
    public string? Title { get; set; }
    public bool LikedByUser { get; set; }
    public int LikesCount { get; set; }  // счетчик для каждой карточки
    // public bool IsGuest { get; set; }
    public string? Username { get; set; } // Добавляем имя автора
    public bool IsOnline { get; set; } = false;
}
// public class CreatePostDto //для создания постов
// {
//     public Guid PostId { get; set; }
//     public Guid UserId { get; set; }
//     public required string Text { get; set; }
//     public string? Title { get; set; }
//     public bool LikedByUser { get; set; }
//     public bool IsGuest { get; set; }
// }
public class LikePostDto //для переключения лайка на посте
{
    public Guid PostId { get; set; }
}



public class ProfileInfoDto //Для информации о профиле
{
    public required string Username { get; set; }
    public Guid UserId { get; set; } //---------для проверки прав доступа
    //     public string? AvatarUrl { get; set; }
    public string? Header { get; set; }
    //     public string? Location { get; set; }
    public string? Description { get; set; }

    // public bool IsGuest { get; set; } 
    public int PostCount { get; set; } // счетчик перенести в META
    public int LikesCount { get; set; } // счетчик лайков перенести в META
    //     // public DateTime CreatedAt { get; set; }
    //     public bool IsOnline { get; set; } = false;
}


public class PagedResponse<T>
{
    // public int UsersCount { get; set; }
    public IEnumerable<T> Items { get; set; } = [];
    public MetaData Meta { get; set; } = new MetaData();
}
public class MetaData
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

// public class ViewUserCardDto // это карточки с пользователяи, пока рано
// {
//     public bool IsGuest { get; set; }
//     public required Guid UserId { get; set; } // добавляем UserId
//     public string? Username { get; set; } // добавляем Username
//     public Guid GuidId { get; set; }
//     public string Name { get; set; }
//     public string? Description { get; set; } // добавляем Description
//     public int PostCount { get; set; }
//     public int LikeCount { get; set; }
//     public DateTime CreatedAt { get; set; }
//     public bool IsOnline { get; set; } = false;
//     public List<Post> Posts { get; private set; } = [];
// }


// public class UserPostDto
// {
//     //кнопка редактирования поста
//     public required string Username { get; set; }
//     public Guid PostId { get; set; }
//     public Guid UserId { get; set; }
//     public required string Text { get; set; } = string.Empty;// возможный черновик
//     public string? Title { get; set; }
//     public bool LikedByUser { get; set; }
//     public bool IsGuest { get; set; }
//     // public DateTime CreatedAt { get; set; }
// }