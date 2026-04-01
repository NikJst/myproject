namespace Testing3;
using Testing3.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Forms;

public interface IPostService
{
    Task<Post> CreatePostAsync(string text, string? title = null, Guid? userId = null); // дописать dto на проверку гостя для черновика
    void DeletePost(Guid postId);
    Post? GetPost(Guid postId);
    PagedResponse<ViewPostsDto> GetAllPosts(int PageNumber, int PageSize, Guid? userId = null);
    Task<List<ViewPostsDto>> GetAllPostsForUserAsync(Guid userId);
}
public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly ApplicationDbContext _dbcontext;

    public PostService(ApplicationDbContext dbContext, ILogger<PostService> logger)
    {
        _dbcontext = dbContext;
        _logger = logger;
    }
    public async Task<Post> CreatePostAsync(string text, string? title = null, Guid? userId = null)
    {
        var post = new Post(text, userId ?? Guid.Empty, title);
        await _dbcontext.Posts.AddAsync(post);
        await _dbcontext.SaveChangesAsync();
        return post;
    }

    public Post? GetPost(Guid postId)
    {
        _logger.LogInformation($"Getting post with ID: {postId}");
        return _dbcontext.Posts.FirstOrDefault(p => p.PostId == postId);
    }

    public PagedResponse<ViewPostsDto> GetAllPosts(int PageNumber, int PageSize, Guid? userId = null)
    {
        _logger.LogInformation("Getting all posts");

        var items = _dbcontext.Posts
        .OrderBy(p => p.CreatedAt)
        .Include(p => p.User)
        .Select(p => new ViewPostsDto
        {
            UserId = p.UserId,
            PostId = p.PostId,
            Text = p.Text,
            Title = p.Title,
            LikedByUser = p.Likes.Any(l => l.UserId == userId),
            LikesCount = p.Likes.Count,
            Username = p.User.Username,
            CreatedAt = p.CreatedAt
        })
        // .GroupBy(p => p.UserId) вот так можно сгруппировать по пользователю
        .Skip((PageNumber - 1) * PageSize)
        .Take(PageSize)
        .ToList();

        return new PagedResponse<ViewPostsDto>
        {
            Items = items,
            Meta = new MetaData
            {
                TotalCount = _dbcontext.Posts.Count(),
                PageNumber = PageNumber,
                PageSize = PageSize
            }

        };
    }

    public async Task<List<ViewPostsDto>> GetAllPostsForUserAsync(Guid userId)
    {

        var posts = await _dbcontext.Posts
        .Where(p => p.UserId == userId)
        .OrderBy(p => p.CreatedAt)
        .Include(p => p.Likes) // включить связанные объекты
        .Include(p => p.User) // включить данные пользователя
        .Select(p => new ViewPostsDto // мы выбираем что отдавать клиенту
        {
            UserId = p.UserId,
            PostId = p.PostId,  // Изменено с GuidId на PostId
            Text = p.Text,
            Title = p.Title,
            LikedByUser = p.Likes.Any(l => l.UserId == userId), //измененный и правильный вариант
            LikesCount = p.Likes.Count,  // Добавляем подсчет лайков
            Username = p.User.Username,  // Добавляем имя автора
            CreatedAt = p.CreatedAt
            // Мы больше не лезем в _dbcontext.Likes вручную!
            // Мы спрашиваем СУБЪЕКТИВНО у поста: "Есть ли среди ТВОИХ лайков мой?
        })
        .ToListAsync();

        return posts;

    }
    public void DeletePost(Guid postId)
    {
        _logger.LogInformation($"Deleting post with ID: {postId}");
        var post = _dbcontext.Posts.FirstOrDefault(p => p.PostId == postId);
        if (post != null)
        {
            _dbcontext.Posts.Remove(post);
            _dbcontext.SaveChanges();
        }
    }
}