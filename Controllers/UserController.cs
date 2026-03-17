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
    public IActionResult GetOrCreateUser()
    {
        return Ok(_userService.GetOrCreateUser(HttpContext));
    }

    [HttpGet("allUsers")]
    public IActionResult GetAllUsers()
    {
        return Ok(_userService.GetAllUsers());
    }

}