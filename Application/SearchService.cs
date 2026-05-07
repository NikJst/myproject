using Microsoft.EntityFrameworkCore;
using Testing3.DTO;
using Testing3;
namespace Testing3.Application;

public interface ISearch
{
    Task<List<ViewPostsDto>> SearchToWords(string query);
}

public class Search : ISearch
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILogger<Search> _logger;
    public Search(ApplicationDbContext dbcontext, ILogger<Search> logger)
    {
        _dbcontext = dbcontext;
        _logger = logger;
    }
    public async Task<List<ViewPostsDto>> SearchToWords(string query)
    {
        var results = await _dbcontext.Posts.FromSql($@"
                SELECT ""Id"", ""Text"", ""UserId"", ""CreatedAt""
                FROM ""Posts"" 
                WHERE to_tsvector('russian', ""Text"") @@ websearch_to_tsquery('russian', {query})
                ORDER BY ""CreatedAt"" DESC")
            .Include(p => p.Likes)
            .Select(p => new ViewPostsDto
            {
                PostId = p.Id,
                UserId = p.UserId,
                Text = p.Text,
                IsOnline = true //временно
            })
            .ToListAsync();

        return results;

    }
}