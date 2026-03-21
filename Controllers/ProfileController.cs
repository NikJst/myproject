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
    [Route("info")] //username может изменяться далее так что лучше не использовать его в пути
    public async Task<IActionResult> PatchInfo([FromBody] ProfileInfoDto profileDto)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);
        var updatedProfile = await profileService.PatchInfo(profileDto, user);
        return Ok(updatedProfile);
    }

    [HttpGet]
    [Route("{username}/info")] // должен вернуть кнопку редактирования если это наш профиль
    public async Task<IActionResult> GetUserProfile([FromRoute] string username)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);

        var profileInfo = await profileService.GetProfileInfo(username, user);
        return Ok(profileInfo);
    }

    //------------>
    [HttpGet]
    [Route("{username}/posts")]
    public async Task<IActionResult> GetUserPosts([FromRoute] string username)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);

        var userPosts = await profileService.GetUserPosts(username, user);
        return Ok(userPosts);
    }
}
