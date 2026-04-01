using Microsoft.AspNetCore.Mvc;
using Testing3.Application;
using Microsoft.EntityFrameworkCore;

namespace Testing3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OnlineController : ControllerBase
{
    private readonly IOnlineService _onlineService;
    private readonly IUserService _userService;
    private readonly IPostService _postService;
    private readonly ILogger<OnlineController> logger;

    public OnlineController(IOnlineService onlineService, IUserService userService, IPostService postService, ILogger<OnlineController> logger)
    {
        _onlineService = onlineService;
        _userService = userService;
        _postService = postService;
        this.logger = logger;


    }

    [HttpPost]
    public IActionResult Ping()
    {
        logger.LogInformation("Ping endpoint called");

        var userId = Request.Cookies["UserId"];
        logger.LogInformation("UserId from cookie: {UserId}", userId);

        if (string.IsNullOrEmpty(userId))
        {
            var user = _userService.GetUserIdFromCookie(HttpContext);
            userId = user.ToString();
            logger.LogInformation("UserId from service: {UserId}", userId);
        }

        _onlineService.SetUserOnline(userId!);
        logger.LogInformation("User {UserId} set as online", userId);

        return Ok("Online");
    }

    [HttpGet("users")]
    public IActionResult GetOnlineUsers([FromQuery] List<string> postIds)
    {
        if (postIds == null || postIds.Count == 0)
        {
            return StatusCode(500, "No post IDs provided");
        }
        var status = _onlineService.GetOnlineUsers(postIds);//отдаем словарик
        logger.LogWarning("получены онлайн пользователи в словаре");


        return Ok(status);
    }
}
