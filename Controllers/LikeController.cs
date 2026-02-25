using Microsoft.AspNetCore.Mvc;
namespace Testing3.Controllers;

public class LikePostDto
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
}
[ApiController]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly ILikePost likePost;
    private readonly IGuestService guestService;

    public LikeController(ILikePost likePost, IGuestService guestService)
    {
        this.likePost = likePost;
        this.guestService = guestService;
    }

    [HttpPost]
    public IActionResult ToggleLike([FromBody] LikePostDto dto)
    {
        var user = guestService.GetOrCreateGuest(HttpContext);// получаем объект гостя из куки или создаем новый

        try
        {
            likePost.SetLikePost(dto.UserId, dto.PostId);
        }
        catch (InvalidOperationException)
        {
            likePost.RemoveLikePost(dto.UserId, dto.PostId);
        }

        var count = likePost.GetLikeCount(dto.PostId);
        return Ok(new { LikesCount = count });
    }
}
