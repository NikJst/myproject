using Microsoft.AspNetCore.Mvc;
namespace Testing3;
public class PostDto //DTO
{
    public string Text { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool LikedByUser { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IGuestService _guestService;

    public PostController(IPostService postService, IGuestService guestService)
    {
        _postService = postService;
        _guestService = guestService;
    }

    [HttpPost]
    public IActionResult CreatePost([FromBody] PostDto request)
    {

        try
        {
            _postService.CreatePost(request.Text, request.UserId);
            Console.WriteLine("Post created"); // для отладки
            return Ok(request);
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
