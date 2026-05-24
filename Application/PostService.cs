namespace Testing3.Application;
using Testing3.DTO;
using Testing3;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

public interface IPostService
{
    Task<ViewPostDto> CreatePostAsync(string text, Guid userId, string Username, bool IsPublished); // дописать dto на проверку гостя для черновика
    void DeletePost(Guid postId, Guid userId);
    Post? GetPost(Guid postId);
    PagedResponse<ViewPostDto> GetAllPosts(int PageNumber, int PageSize, Guid? userId = null);
    Task<List<ViewPostDto>> GetUserPostsAsync(Guid userId);
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
    public async Task<ViewPostDto> CreatePostAsync(string text, Guid userId, string Username, bool IsPublished)
    {
        var post = new Post(text, userId, IsPublished);
        await _dbcontext.Posts.AddAsync(post);
        await _dbcontext.SaveChangesAsync();
        var responseDto = new ViewPostDto
        {
            PostId = post.Id,
            Title = post.Title,
            Text = post.Text,
            UserId = post.UserId,
            Username = Username,
            LikesCount = 0,
            CreatedAt = post.CreatedAt,
            IsMyPost = true,
            IsPublished = IsPublished,
            MyLike = false,
        };
        return responseDto;
    }

    public Post? GetPost(Guid postId)
    {

        return _dbcontext.Posts.FirstOrDefault(p => p.Id == postId);
    }

    public PagedResponse<ViewPostDto> GetAllPosts(int PageNumber, int PageSize, Guid? userId = null)
    {
        var post = _dbcontext.Posts
                .Where(p => p.IsPublished == true) //исключать черновики
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.User)
                .Select(p => new ViewPostDto
                {
                    UserId = p.UserId,
                    PostId = p.Id,
                    Text = p.Text,
                    Title = p.Title,

                    Username = p.User.Username,
                    CreatedAt = p.CreatedAt,
                    MyLike = p.Likes.Any(l => l.UserId == userId),
                    MyBookmark = p.Bookmarks.Any(b => b.UserId == userId),
                    IsMyPost = p.UserId == userId,
                    LikesCount = p.Likes.Count // если IsMyPost == true то отображать количество лайков
                })
                // .GroupBy(p => p.UserId) вот так можно сгруппировать по пользователю
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

        return new PagedResponse<ViewPostDto>
        {
            Items = post.Result,
            Meta = new MetaData
            {
                TotalCount = _dbcontext.Posts.Count(),
                PageNumber = PageNumber,
                PageSize = PageSize
            }

        };
    }

    public async Task<List<ViewPostDto>> GetUserPostsAsync(Guid userId)
    {

        var post = await _dbcontext.Posts
        .Where(p => p.UserId == userId) // наши посты 
        .OrderByDescending(p => p.CreatedAt)
        .Include(p => p.User)
        .Select(p => new ViewPostDto
        {
            UserId = p.UserId,
            PostId = p.Id,
            Text = p.Text,
            Title = p.Title,
            Username = p.User.Username,
            CreatedAt = p.CreatedAt,
            MyLike = p.Likes.Any(l => l.UserId == userId),
            MyBookmark = p.Bookmarks.Any(b => b.UserId == userId),
            IsMyPost = p.UserId == userId,
            LikesCount = p.Likes.Count
            // это dto твоих постов, тут кнопки редактирования и удаления
        })
        .ToListAsync();

        return post;

    }
    public void DeletePost(Guid postId, Guid userId)
    {
        _dbcontext.Posts.Remove(_dbcontext.Posts.First(p => p.Id == postId && p.UserId == userId));
        _dbcontext.SaveChanges();
    }
}