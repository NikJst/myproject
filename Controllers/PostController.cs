using Microsoft.AspNetCore.Mvc;
namespace Testing3;
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IGuestService _guestService;
    private readonly ILogger<PostController> _logger;

    public PostController(IPostService postService, IGuestService guestService, ILogger<PostController> logger)
    {
        _postService = postService;
        _guestService = guestService;
        _logger = logger;
    }

    [HttpPost]
    public IActionResult CreatePost([FromBody] CreatePostDto request)
    {

        try
        {
            var user = _guestService.GetOrCreateGuest(HttpContext);
            _postService.CreatePost(request.Text, user.GuidId);
            _logger.LogInformation("Post created with user ID: {UserId}", user.GuidId);
            return Ok(new { Text = request.Text, GuidId = user.GuidId });
        }
        catch (Exception ex)
        {
            // Возвращаем 500 + текст ошибки
            return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
        }
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

    [HttpGet]
    public IActionResult GetAllPosts()
    {
        var user = _guestService.GetOrCreateGuest(HttpContext);
        
        var postDtos = _postService.GetAllPostsForUser(user.GuidId);
        return Ok(postDtos);
        // var posts = _postService.GetAllPosts();
        // return Ok(posts);
    }
}
