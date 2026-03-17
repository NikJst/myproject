using Microsoft.AspNetCore.Mvc;
using Testing3;
using Testing3.DTO;
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostController> _logger;
    private readonly IUserService _userService;

    public PostController(IPostService postService, IUserService userService, ILogger<PostController> logger)
    {
        _postService = postService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto request)
    {

        try
        {
            var user = await _userService.GetOrCreateUser(HttpContext);

            var post = await _postService.CreatePostAsync(request.Text, user.GuidId, request.Title);

            _logger.LogInformation($"Post создан with user ID: {user.GuidId}");

            return Ok(new
            {
                Text = request.Text,
                GuidId = post.GuidId,
                UserId = user.GuidId,
                Title = request.Title
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
        }
    }

    [HttpDelete("{postId}")]
    public IActionResult DeletePost(Guid postId)
    {
        _postService.DeletePost(postId);
        return Ok();
    }


    [HttpGet]
    public IActionResult GetAllPosts()
    {
        var user = _guestService.GetOrCreateGuest(HttpContext);

        var dto = _postService.GetAllPostsForUser(user.GuidId);
        return Ok(dto);
    }

    [HttpGet("{postId}")]
    public IActionResult GetPost(Guid postId)
    {
        var post = _postService.GetPost(postId);
        if (post == null) return NotFound();
        return Ok(post);
    }
}
