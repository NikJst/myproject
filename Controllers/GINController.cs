using Microsoft.AspNetCore.Mvc;
using Testing3.Application;
using Testing3.DTO;
namespace Testing3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GINController : ControllerBase
{
    private readonly ISearch _search;
    private readonly ILogger<GINController> _logger;
    public GINController(ISearch search, ILogger<GINController> logger)
    {
        _search = search;
        _logger = logger;
    }

    [HttpPost]
    public async Task<List<ViewPostsDto>> Search([FromQuery] string query)
    {
        _logger.LogWarning("Запрос: {Query}", query);
        if (string.IsNullOrWhiteSpace(query))
        {
            _logger.LogWarning("Invalid query");
            throw new ArgumentException("invalid query");
        }
        var result = await _search.SearchToWords(query);
        return result;
    }
}