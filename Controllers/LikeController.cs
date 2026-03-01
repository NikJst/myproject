using Microsoft.AspNetCore.Mvc;
namespace Testing3.Controllers;

public class LikePostDto
{
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
            likePost.SetLikePost(user.GuidId, dto.PostId);
        }
        catch (InvalidOperationException)
        {
            likePost.RemoveLikePost(user.GuidId, dto.PostId);
        }

        var count = likePost.GetLikeCount(dto.PostId);
        return Ok(new { LikesCount = count });
    }
    
    [HttpGet("post/{postId}")]
    public IActionResult GetLikesInfo(Guid postId)
    {
        var user = guestService.GetOrCreateGuest(HttpContext);
        var likedByUser = likePost.IsLikedByUser(user.GuidId, postId);

        var count = likePost.GetLikeCount(postId);

        return Ok(new
        {
            LikesCount = count,
            LikedByUser = likedByUser
        });
    }
}
