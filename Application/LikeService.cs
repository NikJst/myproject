namespace Testing3;

public interface ILikeService
{
    void SetLikePost(Guid userId, Guid postId);
    void RemoveLikePost(Guid userId, Guid postId);
    int GetLikeCount(Guid postId);
    bool IsLikedByUser(Guid userId, Guid postId);
}

public class LikePost : ILikeService
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
            _logger.LogWarning($"Лайк уже существует для пользователя {userId} и поста {postId}");
            throw new InvalidOperationException("Лайк уже существует");
        }
        var like = new Like(userId, postId);
        _repo.Add(like);
        _logger.LogInformation($"Пользователь {userId} создал new Like и добавил его к посту {postId}");
    }

    public void RemoveLikePost(Guid userId, Guid postId)
    {
        if (!_repo.Exists(userId, postId))
            return;

        _repo.Remove(userId, postId);
        _logger.LogInformation($"Пользователь удалил лайк с поста");
    }

    public int GetLikeCount(Guid postId)
    {
        var count = _repo.GetLikeCount(postId);
        _logger.LogInformation($"Теперь пост имеет {count} лайков");
        return count;
    }

    public bool IsLikedByUser(Guid userId, Guid postId)
    {
        _logger.LogInformation($"Проверка лайка пользователя {userId} для поста {postId}");
        var isLiked = _repo.Exists(userId, postId);
        _logger.LogInformation($"Пользователь лайкнул пост {postId}: {isLiked}");
        return isLiked;
    }
}