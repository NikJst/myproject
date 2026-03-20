using Testing3.DTO;
using Microsoft.EntityFrameworkCore;
namespace Testing3.Application;

public interface IProfileService
{
    Task<ProfileDto> PatchBio(Guid userId, ProfileDto profileDto);
    Task<ProfileDto> GetProfileContent(string username, User user);
    // Task<ProfileDto> GetUserProfileContent(Guid userId);
}

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<ProfileService> logger;
    public ProfileService(ApplicationDbContext dbContext, ILogger<ProfileService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }
    public async Task<ProfileDto> PatchBio(Guid userId, ProfileDto profileDto)
    {
        try
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");
            logger.LogError("User found: {UserId}", user.UserId);

            user.Username = profileDto.Username;
            user.Header = profileDto.Header;
            user.Description = profileDto.Description;
            // user.Location = profileDto.Location;

            await dbContext.SaveChangesAsync();
            logger.LogWarning($"Profile обновлен в бд");
            return new ProfileDto
            {
                Username = user.Username,
                Header = user.Header,
                Description = user.Description,
                // Location = user.Location
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating profile");
            throw;
        }
        //дописать дату и др обязательные поля потом 
    }


    //-------->
    public async Task<ProfileDto> GetProfileContent(string username, User user)
    // таким образом получается что у нас один объект ProfileDto для всех операций, но с разным содержимым
    {
        var usernameUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username);

        if (usernameUser == null) throw new Exception("User not found");
        logger.LogWarning("User found: " + usernameUser);

        if (user.UserId == usernameUser.UserId)
        {
            // Считаем посты пользователя
            var postCount = await dbContext.Posts.CountAsync(p => p.UserId == user.UserId);
            user.PostCount = postCount;//счетчик постов как значение в бд. Денормализация для отображения в профиле
            await dbContext.SaveChangesAsync();


            var UserProfileDto = new ProfileDto
            {
                Username = user.Username,
                Header = user.Header,
                Description = user.Description,
                IsMine = true,
                PostCount = postCount
            };
            return UserProfileDto;
        }

        // Считаем посты для чужого профиля
        var postCountPublic = await dbContext.Posts.CountAsync(p => p.UserId == usernameUser.UserId);

        var PublicProfileDto = new ProfileDto
        {
            Username = usernameUser.Username,
            Header = usernameUser.Header,
            Description = usernameUser.Description,
            IsMine = false,
            PostCount = postCountPublic
        };
        return PublicProfileDto;
    }
}