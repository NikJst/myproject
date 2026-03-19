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
            var UserProfileDto = new ProfileDto
            {
                Username = user.Username,
                // Header = user.Header,
                // Description = user.Description,
                // Location = user.Location,
                // IsGuest = user.IsGuest,
                // UserId = user.UserId,
                // PostCount = user.PostCount,
                // LikeCount = user.LikeCount,
                // CreatedAt = user.CreatedAt,
                // IsOnline = user.IsOnline,
                // Posts = user.Posts,
                // Likes = user.Likes, // пока не уверен
                IsMine = true
            };
            return UserProfileDto;
        }

        var PublicProfileDto = new ProfileDto
        {
            Username = usernameUser.Username,
            // Header = usernameUser.Header,
            // Description = usernameUser.Description,
            // // Location = usernameUser.Location,
            // // IsGuest = usernameUser.IsGuest,
            // // GuidId = usernameUser.UserId,
            // PostCount = usernameUser.PostCount,
            // LikeCount = usernameUser.LikeCount,
            // CreatedAt = usernameUser.CreatedAt,
            // IsOnline = usernameUser.IsOnline,
            // Posts = usernameUser.Posts,
            // Likes = usernameUser.Likes // пока не уверен
            IsMine = false
        };
        return PublicProfileDto;
    }
}