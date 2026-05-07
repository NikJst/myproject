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
    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetTargetProfile([FromRoute] string username)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);

        var profileInfo = await profileService.GetTargetProfile(username, user);
        return Ok(profileInfo);
    }

    [HttpPatch]
    [Route("{username}/edit")] //username может изменяться далее так что лучше не использовать его в пути
    public async Task<IActionResult> PatchInfo([FromRoute] string username, [FromBody] ProfileInfoDto profileDto)
    {
        //предположим что пользователь уже авторизован
        var user = await userService.GetOrCreateUser(HttpContext);
        var updatedProfile = await profileService.PatchInfo(profileDto, user);
        return Ok(updatedProfile);
    }

    //===================>
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


    // [HttpGet("{username}/favorites")]
    // public async Task<IActionResult> GetFavorites([FromRoute] string username)
    // {
    //     //предположим что пользователь уже авторизован
    //     var user = await userService.GetOrCreateUser(HttpContext);

    //     var likes = await profileService.GetFavoritesPosts(username, user);
    //     return Ok(likes);
    // }

}
