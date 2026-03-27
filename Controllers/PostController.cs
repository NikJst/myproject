using Microsoft.AspNetCore.Mvc;
using Testing3;
using Testing3.DTO;
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostService _postService;
    private readonly IUserService _userService;

    public PostController(ILogger<PostController> logger, IPostService postService, IUserService userService)
    {
        _logger = logger;
        _postService = postService;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] ViewPostsDto request)
    {

        try
        {
            var user = await _userService.GetOrCreateUser(HttpContext);
            var post = await _postService.CreatePostAsync(request.Text, request.Title, user.UserId);

            _logger.LogInformation($"Post создан with user ID: {user.UserId}");

            return Ok(new ViewPostsDto
            {
                Text = request.Text,
                PostId = post.PostId,
                Title = request.Title,
                UserId = user.UserId,// взяли id из сессии
                Username = user.Username,
                LikedByUser = false, // Только что созданный пост не может быть лайкнут тем же пользователем
                LikesCount = 0, // У нового поста 0 лайков
                // IsGuest = user.Username.StartsWith("Guest_") // Определяем гость ли пользователь
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var user = await _userService.GetOrCreateUser(HttpContext);

        var dto = await _postService.GetAllPostsForUserAsync(user.UserId);
        return Ok(dto);
    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPosts(Guid userId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var dto = await _postService.GetAllPostsForUserAsync(userId);
        return Ok(dto);
    }

    [HttpDelete("{postId}")]
    public IActionResult DeletePost(Guid postId)
    {
        _postService.DeletePost(postId);
        return Ok();
    }

    [HttpGet("{postId}")]
    public IActionResult GetPost(Guid postId)
    {
        var post = _postService.GetPost(postId);
        if (post == null) return NotFound();
        return Ok(post);
    }
}
