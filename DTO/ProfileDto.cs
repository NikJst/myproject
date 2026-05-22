namespace Testing3.DTO;

public class ProfileInfoDto //Для информации о профиле
{
    public required string Username { get; set; }
    public Guid UserId { get; set; }
    //     public string? AvatarUrl { get; set; }
    public string? Header { get; set; }
    //     public string? Location { get; set; }
    public string? Description { get; set; }

    // public bool IsGuest { get; set; } 
    public int PostCount { get; set; } // счетчик перенести в META
    public int LikesCount { get; set; } // счетчик лайков перенести в META
                                        //     // public DateTime CreatedAt { get; set; }
    public bool IsOnline { get; set; } = false;
    public int Age { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Hobby { get; set; }
    public string? Interests { get; set; }

}
public class ProfileEditDto
{
    public string? Header { get; set; }
    //     public string? Location { get; set; }
    public string? Description { get; set; }

    public int Age { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Hobby { get; set; }
    public string? Interests { get; set; }
}
