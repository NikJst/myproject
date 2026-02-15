namespace Testing3;
public interface ILikeRepository
{
    bool Exists(Guid userId, Guid postId);
    void Add(Like like);
    void Remove(Guid userId, Guid postId);
    int GetLikeCount(Guid postId);
}