using Microsoft.AspNetCore.Mvc;
namespace Testing3.Controllers;
[ApiController]
[Route("guest")]
public class GuestController : Controller
{
    private readonly IGuestService guest;

    public GuestController(IGuestService guestService)
    {
        guest = guestService;
    }

    [HttpGet]
    public IActionResult GetGuest()
    {
        return Ok(guest.GetOrCreateGuest(HttpContext));
    }

}