namespace Testing3;

public interface ILikePost
{
    void SetLikePost(Guid userId, Guid postId);
    void RemoveLikePost(Guid userId, Guid postId);
    int GetLikeCount(Guid postId);
    bool IsLikedByUser(Guid userId, Guid postId);
}

public class LikePost : ILikePost
{
    private readonly ILogger<LikePost> _logger;
    private readonly ILikeRepository _repo;
    public LikePost(ILikeRepository repo, ILogger<LikePost> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public void SetLikePost(Guid userId, Guid postId)
    {
        if (_repo.Exists(userId, postId))
        {
            _logger.LogWarning("Лайк уже существует для пользователя {UserId} и поста {PostId}", userId, postId);
            throw new InvalidOperationException("Лайк уже существует");
        }
        var like = new Like(userId, postId);
        _repo.Add(like);
        _logger.LogInformation("Пользователь {UserId} создал new Like и добавил его к посту {PostId}", userId, postId);
    }

    public void RemoveLikePost(Guid userId, Guid postId)
    {
        if (!_repo.Exists(userId, postId))
            return;

        _repo.Remove(userId, postId);
        _logger.LogInformation("Пользователь {UserId} удалил лайк с поста {PostId}", userId, postId);
    }

    public int GetLikeCount(Guid postId)
    {
        var count = _repo.GetLikeCount(postId);
        _logger.LogInformation("Пост {PostId} имеет {Count} лайков", postId, count);
        return count;
    }

    public bool IsLikedByUser(Guid userId, Guid postId)
    {
        _logger.LogInformation("Проверка лайка пользователя {UserId} для поста {PostId}", userId, postId);
        var isLiked = _repo.Exists(userId, postId);
        _logger.LogInformation("Пользователь {UserId} лайкнул пост {PostId}: {IsLiked}", userId, postId, isLiked);
        return isLiked;
    }
}