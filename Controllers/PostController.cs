using Microsoft.AspNetCore.Mvc;
using Testing3;
using Testing3.DTO;
using Testing3.Application;
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
            _logger.LogInformation("CreatePost called. IsPublished value: {IsPublished}", request.IsPublished);
            _logger.LogInformation("Full request object: {@Request}", request);

            var user = await _userService.GetOrCreateUser(HttpContext);
            var response = await _postService.CreatePostAsync(request.Text, user.Id, user.Username, request.IsPublished ?? false); // убрал заголовок
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts([FromQuery] int? PageNumber, [FromQuery] int? PageSize)
    {
        var user = await _userService.GetOrCreateUser(HttpContext);

        var dto = _postService.GetAllPosts(PageNumber ?? 1, PageSize ?? 10, user.Id);
        return Ok(dto);
    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPosts(Guid userId)
    {
        var pagedResult = await _postService.GetUserPostsAsync(userId);
        return Ok(pagedResult);
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
