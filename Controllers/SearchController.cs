using Microsoft.AspNetCore.Mvc;
using Testing3.Application;
using Testing3.DTO;
namespace Testing3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearch _search;
    private readonly ILogger<SearchController> _logger;
    public SearchController(ISearch search, ILogger<SearchController> logger)
    {
        _search = search;
        _logger = logger;
    }

    [HttpGet]
    public async Task<List<ViewPostsDto>> Search([FromQuery] string query)
    {
        _logger.LogWarning($"Запрос: {query}", query);
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("invalid query");
        }
        var result = await _search.SearchToWords(query);
        return result;
    }
    [HttpPost("users")]
    public async Task<List<UserFilterTarget>> SearchUsers([FromBody] UserSearchRequest request)
    {
        var result = await _search.SearchUsers(request.Range, request.Values);
        return result;
    }
}