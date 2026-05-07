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

        var userId = Request.Cookies["GuestId"];
        // logger.LogInformation("UserId from cookie: {UserId}", userId);

        if (string.IsNullOrEmpty(userId))
        {
            var user = _userService.GetUserIdFromCookie(HttpContext);
            userId = user.ToString();
            // logger.LogWarning("UserId не был найден в куках, использован из сервиса: {UserId}", userId);
        }

        _onlineService.SetUserOnline(userId!);
        // logger.LogInformation("User {UserId} set as online", userId);

        return Ok("Пинг на сервер получен");
    }

    [HttpGet("users")]
    public IActionResult GetOnlineUsers([FromQuery] string userIds)
    {
        if (string.IsNullOrEmpty(userIds))
        {
            // logger.LogWarning("GetOnlineUsers: No user IDs provided");
            return StatusCode(500, "No user IDs provided");
        }

        // Разделяем строку с запятыми на массив ID
        var idList = userIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        // logger.LogInformation("GetOnlineUsers: Received {Count} user IDs: {UserIds}", idList.Count, string.Join(", ", idList));

        var status = _onlineService.GetOnlineUsers(idList);//отдаем словарик
        // logger.LogWarning("получены онлайн пользователи в словаре");
        // logger.LogInformation("GetOnlineUsers: Returning {Count} statuses: {Statuses}", status.Count, string.Join(", ", status.Select(kvp => $"{kvp.Key}={kvp.Value}")));

        // Добавляем свой заголовок (обычно начинаются с X-)
        // Response.Headers.Append("X-Debug-Message", "Online users retrieved successfully:" + " количество: " + status.Count);

        return Ok(status);
    }
}
