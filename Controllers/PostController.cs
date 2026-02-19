using Microsoft.AspNetCore.Mvc;
namespace Testing3;
//===============>DTO
public class CreatePostRequest
{
    public string Text { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public IActionResult CreatePost([FromBody] CreatePostRequest request)
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
        var posts = _postService.GetAllPosts();
        return Ok(posts);
    }
}
