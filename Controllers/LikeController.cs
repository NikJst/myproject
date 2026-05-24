using Microsoft.AspNetCore.Mvc;
using Testing3.DTO;
namespace Testing3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly ILikeService _likePost;
    private readonly IUserService _userService;
    private readonly ILogger<LikeController> _logger;

    public LikeController(ILikeService likePost, IUserService userService, ILogger<LikeController> logger)
    {
        _likePost = likePost;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ToggleLike([FromBody] TogglePostDto dto)
    {
        var user = await _userService.GetOrCreateUser(HttpContext);// получаем объект UserId из куки или создаем новый

        try
        {
            await _likePost.SetLikePost(user.Id, dto.PostId);
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("Удаляем лайк");
            await _likePost.RemoveLikePost(user.Id, dto.PostId);
        }

        var count = await _likePost.GetLikeCount(dto.PostId);
        var likedByUser = await _likePost.IsLikedByUser(user.Id, dto.PostId);
        return Ok(new { likesCount = count, likedByUser = likedByUser }); // Добавляем информацию о состоянии лайка
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetLikesInfo(Guid postId)
    {
        var user = await _userService.GetOrCreateUser(HttpContext);

        var likedByUser = await _likePost.IsLikedByUser(user.Id, postId);

        var count = await _likePost.GetLikeCount(postId);

        return Ok(new
        {
            likesCount = count,
            likedByUser = likedByUser
        });
    }
}
