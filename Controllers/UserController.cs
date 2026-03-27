using Microsoft.AspNetCore.Mvc;
namespace Testing3.Controllers;
[ApiController]
[Route("User")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrCreateUser()
    {
        var user = await _userService.GetOrCreateUser(HttpContext);
        return Ok(user);
    }

    /* [HttpGet("allUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }*/

}