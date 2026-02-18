// namespace Testing3;
// public class LikePost
// {
//     private readonly ILikeRepository _repo;

//     public LikePost(ILikeRepository repo)
//     {
//         _repo = repo;
//     }

//     public void SetLikePost(Guid userId, Guid postId)
//     {
//         if (_repo.Exists(userId, postId))
//         {
//             Console.WriteLine("вызван Exists, лайк уже существует");
//             throw new InvalidOperationException("Like already exists");
//         }
//         var like = new Like(userId, postId);
//         _repo.Add(like);
//         Console.WriteLine($"User {userId} liked post {postId}");
//     }

//     public void RemoveLikePost(Guid userId, Guid postId)
//     {
//         if (!_repo.Exists(userId, postId))
//         {
//             throw new InvalidOperationException("Like does not exist");
//         }
//         _repo.Remove(userId, postId);
//         Console.WriteLine($"User {userId} unliked post {postId}");
//     }

//     public int GetLikeCount(Guid postId)
//     {
//         return _repo.GetLikeCount(postId);
//     }
// }