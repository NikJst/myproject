using Microsoft.AspNetCore.Mvc;
namespace Testing3;
using Testing3.DTO;
using Testing3.Application;

[ApiController]
[Route("api/[controller]")] //api/Profile
public class ProfileController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<ProfileController> logger;
    private readonly IProfileService profileService;

    public ProfileController(IUserService userService, ILogger<ProfileController> logger, IProfileService profileService)
    {
        this.userService = userService;
        this.logger = logger;
        this.profileService = profileService;
    }

    [HttpPatch]
    [Route("{username}")] //для создания профиля. В будущем должно быть переброшено на страницу регистрации
    public async Task<IActionResult> PatchBio([FromBody] ProfileDto profileDto)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);
        var updatedProfile = await profileService.PatchBio(user.UserId, profileDto);
        return Ok(updatedProfile);
    }

    [HttpGet]
    [Route("{username}/content")] //для получения контента профиля
    public async Task<IActionResult> GetUserProfile([FromRoute] string username)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);//здесь наше все (гостевое)
        // user.UserId;
        // вызываем метод публичного отображения
        //username в дальнейшем используется для поиска id пользователя в бд
        var Content = await profileService.GetProfileContent(username, user);
        return Ok(Content);
    }
}
