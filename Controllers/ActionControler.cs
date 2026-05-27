using Microsoft.AspNetCore.Mvc;
using Testing3.Application;
using Microsoft.EntityFrameworkCore;

namespace Testing3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActionsController : ControllerBase
{
    private readonly IActionService _actionsService;
    private readonly IUserService userService;
    private readonly ILogger<OnlineController> logger;

    public ActionsController(IActionService actionsService, IUserService userService, ILogger<OnlineController> logger)
    {
        this._actionsService = actionsService;
        this.userService = userService;
        this.logger = logger;


    }
    [HttpPost("actions")]
    public async Task<ActionsDto> GetMyAction([FromQuery] string[] ids)
    {
        var me = await userService.GetOrCreateUser(HttpContext);
        var x = await _actionsService.GetMyAction(ids, me);
        return x;

    }
}