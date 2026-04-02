// using Microsoft.EntityFrameworkCore;
// using Testing3.DTO;
// using Testing3;
// namespace Testing3.Application;

// public class Search
// {
//     private readonly ApplicationDbContext _dbcontext;
//     private readonly ILogger<Search> _logger;
//     public Search(ApplicationDbContext dbcontext, ILogger<Search> logger)
//     {
//         _dbcontext = dbcontext;
//         _logger = logger;
//     }
//     public async Task<List<ViewPostsDto>> SearchPosts(string query)
//     {

//         var tsQuery = EF.Functions.WebSearchToTsQuery("russian", query);

//         var result = await _dbcontext.Posts
//             .Where(p => p.search_vector.Matches(tsQuery))
//             .Select(p => new ViewPostsDto //для отображения всех постов
//             {
//                 PostId = p.Id,
//                 UserId = p.UserId,
//                 Text = p.Text,
//                 // Метаданные (подсветка) тоже работают через LINQ
//                 // Snippet = EF.Functions.ToTsHeadline("russian", p.Text, tsQuery)
//                 // В новых версиях Npgsql есть методы для Headline

//             })
//             .ToListAsync();
//         return result;
//     }
// }