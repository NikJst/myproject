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
    [Route("edit")] //username может изменяться далее так что лучше не использовать его в пути
    public async Task<IActionResult> PatchInfo([FromBody] ProfileInfoDto profileDto)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);
        var updatedProfile = await profileService.PatchInfo(profileDto, user);
        return Ok(updatedProfile);
    }

    //------------>
    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetUserProfileDirect([FromRoute] string username)
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
    //дальше по аналогии лайки, закладки и пр. 
    //к прмимеру
    /* 
    [Authorize] // только авторизованные
    [HttpGet("{username}/likes/drafts")]
    public async Task<IActionResult> GetLikes([FromRoute] string username)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);

        var likes = await profileService.GetLikes(username, user);
        return Ok(likes);
    }
    */
}
