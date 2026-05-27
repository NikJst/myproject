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
    // private readonly IBuilder builder;

    public ProfileController(IUserService userService, ILogger<ProfileController> logger, IProfileService profileService)
    {
        this.userService = userService;
        this.logger = logger;
        this.profileService = profileService;

    }
    /*
*/
    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetProfile([FromRoute] string username)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);

        if (user == null)
        {
            return NotFound();
        }
        var profileInfo = await profileService.GetTargetProfile(username, user);
        return Ok(profileInfo);
    }

    [HttpPatch]
    [Route("edit")] //username может изменяться далее так что лучше не использовать его в пути
    public async Task<IActionResult> PatchInfo([FromQuery] string username, [FromBody] ProfileEditDto profileDto)
    {

        var me = await userService.GetOrCreateUser(HttpContext);
        var updatedProfile = await profileService.PatchUserInfo(profileDto, me, username);

        return Ok(updatedProfile);
    }
    [HttpPost("geatusposts")]
    public async Task<IActionResult> Posts([FromQuery] string username, [FromQuery] char i)
    {
        var me = await userService.GetOrCreateUser(HttpContext);
        var res = await profileService.GetUserActivity(me, i, username);
        return Ok(res);
    }

}


/*
[HttpGet]
[Route("{username}/posts")]
public async Task<IActionResult> GetUserPosts([FromRoute] string username)
{
    //предположим что пользователь уже авторизован
    var user = await userService.GetOrCreateUser(HttpContext);

    var userPosts = await profileService.GetTargetUserPosts(username, user);
    return Ok(userPosts);
}

//===================>
// [Authorize] только авторизованные
[HttpGet]
[Route("{username}/likes")]
public async Task<IActionResult> GetLikes([FromRoute] string username)
{
    //предположим что пользователь уже авторизован
    var user = await userService.GetOrCreateUser(HttpContext);

    var likedPosts = await profileService.GetLikesPosts(username, user);
    return Ok(likedPosts);
}

//===================>
// [Authorize] только авторизованные
[HttpGet]
[Route("{username}/drafts")]
public async Task<IActionResult> GetDrafts([FromRoute] string username)
{
    //предположим что пользователь уже авторизован
    var user = await userService.GetOrCreateUser(HttpContext);

    var draftPosts = await profileService.GetDraftsPosts(username, user);
    return Ok(draftPosts);
}


[HttpGet("{username}/bookmarks")]
public async Task<IActionResult> GetBookmarks([FromRoute] string username)
{
    //предположим что пользователь уже авторизован
    var user = await userService.GetOrCreateUser(HttpContext);

    var bookmarks = await profileService.GetBookmarksPosts(username, user);
    return Ok(bookmarks);
}

[HttpPost("{strusername}/targetposts")]

public async Task<IActionResult> GetTargetUserPosts([FromBody] UniversalPostDto universal, [FromRoute] string strusername)
{
    var me = await userService.GetOrCreateUser(HttpContext);

    var targetuserposts = await builder.GetTargetUserPosts(universal, me, strusername);
    return Ok(targetuserposts);
}
*/





