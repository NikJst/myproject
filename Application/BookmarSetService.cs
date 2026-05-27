using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testing3;
public interface IBookmarkService
{
    Task<bool> SetBookmark(Guid userId, Guid postId);
    Task RemoveBookmark(Guid userId, Guid postId);
    Task<bool> IsBookmarkedByUser(Guid userId, Guid postId);
}

public class BookmarkService : IBookmarkService
{
    private readonly ILogger<BookmarkService> _logger;
    private readonly ApplicationDbContext _context;
    public BookmarkService(ILogger<BookmarkService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> SetBookmark(Guid userId, Guid postId)
    {
        var exists = await _context.Bookmarks.AnyAsync(b => b.UserId == userId && b.PostId == postId);
        if (exists)
        {
            await RemoveBookmark(userId, postId);
            return false;
        }

        var bookmark = new Bookmark(userId, postId);
        await _context.Bookmarks.AddAsync(bookmark);
        await _context.SaveChangesAsync();// сохраняем изменения в базе данных
        _logger.LogInformation($"Пользователь {userId} создал new Bookmark и добавил его к посту {postId}");
        return true;
    }

    public async Task RemoveBookmark(Guid userId, Guid postId)
    {
        var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId);
        _context.Bookmarks.Remove(bookmark);// синхронно для change tracker
        await _context.SaveChangesAsync();// асинхронно
        _logger.LogInformation($"Пользователь убрал закладку с поста");

    }

    public async Task<bool> IsBookmarkedByUser(Guid userId, Guid postId)
    {
        var isBookmarked = await _context.Bookmarks.AnyAsync(b => b.UserId == userId && b.PostId == postId);
        _logger.LogInformation($"Пост {postId} содержит закладку: {isBookmarked}");
        return isBookmarked;
    }
}