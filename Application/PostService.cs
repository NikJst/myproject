namespace Testing3.Application;
using Testing3.DTO;
using Testing3;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

public interface IPostService
{
    Task<ViewPostsDto> CreatePostAsync(string text, Guid userId, string Username, bool IsPublished); // дописать dto на проверку гостя для черновика
    void DeletePost(Guid postId, Guid userId);
    Post? GetPost(Guid postId);
    PagedResponse<ViewPostsDto> GetAllPosts(int PageNumber, int PageSize, Guid? userId = null);
    Task<List<ViewPostsDto>> GetUserPostsAsync(Guid userId);
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
    public async Task<ViewPostsDto> CreatePostAsync(string text, Guid userId, string Username, bool IsPublished)
    {
        var post = new Post(text, userId, IsPublished);
        await _dbcontext.Posts.AddAsync(post);
        await _dbcontext.SaveChangesAsync();
        var responseDto = new ViewPostsDto
        {
            PostId = post.Id,
            Title = post.Title,
            Text = post.Text,
            UserId = post.UserId,
            Username = Username,
            LikedByUser = false,
            LikesCount = 0,
            CreatedAt = post.CreatedAt,
            IsPublished = IsPublished,
        };
        return responseDto;
    }

    public Post? GetPost(Guid postId)
    {
        _logger.LogInformation($"Getting post with ID: {postId}");
        return _dbcontext.Posts.FirstOrDefault(p => p.Id == postId);
    }

    public PagedResponse<ViewPostsDto> GetAllPosts(int PageNumber, int PageSize, Guid? userId = null)
    {
        var items = _dbcontext.Posts
                .Where(p => p.IsPublished == true) //исключать черновики
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.User)
                .Select(p => new ViewPostsDto
                {
                    UserId = p.UserId,
                    PostId = p.Id,
                    Text = p.Text,
                    Title = p.Title,
                    LikedByUser = p.Likes.Any(l => l.UserId == userId), //отображаем лайк пользователя
                    LikesCount = p.Likes.Count,
                    Username = p.User.Username,
                    CreatedAt = p.CreatedAt,
                    DeletedButton = p.UserId == userId
                })
                // .GroupBy(p => p.UserId) вот так можно сгруппировать по пользователю
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

        return new PagedResponse<ViewPostsDto>
        {
            Items = items.Result,
            Meta = new MetaData
            {
                TotalCount = _dbcontext.Posts.Count(),
                PageNumber = PageNumber,
                PageSize = PageSize
            }

        };
    }

    public async Task<List<ViewPostsDto>> GetUserPostsAsync(Guid userId)
    {

        var posts = await _dbcontext.Posts
        .Where(p => p.UserId == userId) //посты в нашем профиле
        .OrderByDescending(p => p.CreatedAt)
        .Include(p => p.User)
        .Select(p => new ViewPostsDto
        {
            UserId = p.UserId,
            PostId = p.Id,
            Text = p.Text,
            Title = p.Title,
            LikedByUser = p.Likes.Any(l => l.UserId == userId),
            LikesCount = p.Likes.Count,  // Добавляем подсчет лайков
            Username = p.User.Username,  // Добавляем имя автора
            CreatedAt = p.CreatedAt,
            IsPublished = p.IsPublished, //сообщить о черновке
            // это dto твоих постов, тут кнопки редактирования и удаления
        })
        .ToListAsync();

        return posts;

    }
    public void DeletePost(Guid postId, Guid userId)
    {
        _dbcontext.Posts.Remove(_dbcontext.Posts.First(p => p.Id == postId && p.UserId == userId));
        _dbcontext.SaveChanges();
    }
}