using Microsoft.AspNetCore.Mvc;
using Testing3.ApplicatOnline;
using Testing3;

namespace Testing3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OnlineController : ControllerBase
{
    private readonly IOnlineService _onlineService;
    private readonly IUserService _userService;

    public OnlineController(IOnlineService onlineService, IUserService userService)
    {
        _onlineService = onlineService;
        _userService = userService;
    }

    [HttpPost]
    public IActionResult Ping(HttpContext httpContext)
    {
        var userId = Request.Cookies["UserId"];
        if (string.IsNullOrEmpty(userId))
        {
            var user = _userService.GetUserIdFromCookie(httpContext);
            userId = user.ToString();
        }
        _onlineService.SetUserOnline(userId!);

        return Ok("Online");
    }
}
