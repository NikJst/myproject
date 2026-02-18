using Microsoft.AspNetCore.Mvc;
namespace Testing3;
//===============>доделать DTO
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public IActionResult CreatePost([FromBody] Post post)
    {
        try
        {
            _postService.CreatePost(post.Text, post.UserId);
            return Ok(post);
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
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpGet]
    public IActionResult GetAllPosts()
    {
        var posts = _postService.GetAllPosts();
        return Ok(posts);
    }
}
