using System.Collections.Generic;
using System.Linq;
using System;
namespace Testing3;

public interface ILikeRepository
{
    bool Exists(Guid userId, Guid postId);
    void Add(Like like);
    void Remove(Guid userId, Guid postId);
    int GetLikeCount(Guid postId);
}

public class LikeRepositoryMock : ILikeRepository
{
    private readonly List<Like> _likes = [];

    public bool Exists(Guid userId, Guid postId)
    {
        return _likes.Any(l => l.UserId == userId && l.PostId == postId);
    }

    public void Add(Like like)
    {
        _likes.Add(like);
    }

    public void Remove(Guid userId, Guid postId)
    {
        var existing = _likes.FirstOrDefault(l => l.UserId == userId && l.PostId == postId);
        if (existing != null)
            _likes.Remove(existing);

    }

    public int GetLikeCount(Guid postId)
    {
        return _likes.Count(l => l.PostId == postId);
    }
}