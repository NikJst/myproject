using Microsoft.AspNetCore.Mvc;
using Testing3.Controllers;
using Testing3.DTO;
using Testing3;

[ApiController]
[Route("api/[controller]")]
public class BookMarkController : ControllerBase
{
    private readonly IBookmarkService bookMarkService;
    private readonly IUserService userService;
    private readonly ILogger<BookMarkController> _logger;

    public BookMarkController(IBookmarkService bookMarkService, IUserService userService, ILogger<BookMarkController> logger)
    {
        this.bookMarkService = bookMarkService;
        this.userService = userService;
        this._logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBookmark([FromBody] TogglePostDto dto)
    {
        var user = await userService.GetOrCreateUser(HttpContext);// получаем объект UserId из куки или создаем новый

        var bookmarkedByUser = await bookMarkService.SetBookmark(user.Id, dto.PostId);

        return Ok(new { bookmarkedByUser }); // Добавляем информацию о состоянии закладки
    }

    // [HttpGet("post/{postId}")]
    // public async Task<IActionResult> GetLikesInfo(Guid postId)
    // {
    //     var user = await _userService.GetOrCreateUser(HttpContext);

    //     var likedByUser = await _likePost.IsLikedByUser(user.Id, postId);

    //     var count = await _likePost.GetLikeCount(postId);

    //     return Ok(new
    //     {
    //         likesCount = count,
    //         likedByUser = likedByUser
    //     });
    // }
}
