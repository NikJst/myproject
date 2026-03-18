using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Testing3;

public interface ILikeService
{
    Task SetLikePost(Guid userId, Guid postId);
    Task RemoveLikePost(Guid userId, Guid postId);
    Task<int> GetLikeCount(Guid postId);
    Task<bool> IsLikedByUser(Guid userId, Guid postId);
}

public class LikeService : ILikeService
{
    private readonly ILogger<LikeService> _logger;
    private readonly ApplicationDbContext _context;
    public LikeService(ILogger<LikeService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task SetLikePost(Guid userId, Guid postId)
    {
        if (await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId))
        {
            _logger.LogWarning($"Лайк уже существует для пользователя {userId} и поста {postId}");
            throw new InvalidOperationException("Лайк уже существует");
        }
        var like = new Like(userId, postId);

        await _context.Likes.AddAsync(like);
        await _context.SaveChangesAsync();// сохраняем изменения в базе данных
        _logger.LogInformation($"Пользователь {userId} создал new Like и добавил его к посту {postId}");
    }

    public async Task RemoveLikePost(Guid userId, Guid postId)
    {
        if (await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId))
            return;

        var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        _context.Likes.Remove(like);//Remove(like) – просто помечает объект для удаления. Это не асинхронная операция.
        await _context.SaveChangesAsync();// асинхронно отправляет все изменения (удаление, добавление, обновление) в базу данных.
        _logger.LogInformation($"Пользователь удалил лайк с поста");
    }

    public async Task<bool> IsLikedByUser(Guid userId, Guid postId)
    {
        var isLiked = await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);
        _logger.LogInformation($"Пост {postId} содержит лайк: {isLiked}");
        return isLiked;
    }
    public async Task<int> GetLikeCount(Guid postId)
    {
        var count = await _context.Likes.CountAsync(l => l.PostId == postId);
        _logger.LogInformation($"Пост {postId} имеет {count} лайков");
        return count;
    }

}