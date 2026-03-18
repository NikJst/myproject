using Microsoft.AspNetCore.Mvc;
using Testing3.DTO;
namespace Testing3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly ILikeService _likePost;
    private readonly IUserService _userService;

    public LikeController(ILikeService likePost, IUserService userService)
    {
        _likePost = likePost;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> ToggleLike([FromBody] LikePostDto dto)
    {
        var user = await _userService.GetOrCreateUser(HttpContext);// получаем объект UserId из куки или создаем новый

        try
        {
            _likePost.SetLikePost(user.UserId, dto.PostId);
        }
        catch (InvalidOperationException)
        {
            _likePost.RemoveLikePost(user.UserId, dto.PostId);
        }

        var count = _likePost.GetLikeCount(dto.PostId);
        return Ok(new { LikesCount = count });
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetLikesInfo(Guid postId) //cкорее всего для отображения количества лайков и статуса лайка для текущего 
    {
        var user = await _userService.GetOrCreateUser(HttpContext);

        var likedByUser = _likePost.IsLikedByUser(user.UserId, postId);

        var count = _likePost.GetLikeCount(postId);

        return Ok(new
        {
            LikesCount = count,
            LikedByUser = likedByUser
        });
    }
}
