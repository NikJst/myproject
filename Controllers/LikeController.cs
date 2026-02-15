// using Microsoft.AspNetCore.Mvc;

// namespace Testing3.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class LikeController : ControllerBase
// {
//     private readonly LikePost _likePost;
//     private readonly IGuestService _guestService;

//     public LikeController(LikePost likePost, IGuestService guestService)
//     {
//         _likePost = likePost;
//         _guestService = guestService;
//     }

//     [HttpPost]
//     public IActionResult ToggleLike()
//     {
//         var user = _guestService.GetOrCreateGuest(HttpContext);
//         var postId = Guid.Parse("..."); // ваш пост

//         try
//         {
//             _likePost.SetLikePost(user.GuidId, postId);
//         }
//         catch (InvalidOperationException)
//         {
//             _likePost.RemoveLikePost(user.GuidId, postId);
//         }

//         var count = _likePost.GetLikeCount(postId);
//         return Ok(new { LikesCount = count });
//     }
// }



// // [ApiController]
// // [Route("api/[controller]")]
// // public class LikeController
// // {
// //     private readonly ILikeRepository likeRepository;

// //     public LikeController(ILikeRepository likeRepository)
// //     {
// //         this.likeRepository = likeRepository;
// //     }

// //     [HttpPost("{itemId}")]
// //     public IActionResult ToggleLike(Guid itemId)
// //     {
// //         // Если объекта ещё нет, создаём с лайком
// //         if (!LikesDb.ContainsKey(itemId))
// //             LikesDb[itemId] = 1;
// //         else
// //             LikesDb[itemId] += 1; // увеличиваем счетчик лайков

// //         return Ok(new { LikesCount = LikesDb[itemId] });
// //     }
// // }