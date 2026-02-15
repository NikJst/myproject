using Microsoft.AspNetCore.Http;

namespace Testing3;

public interface IGuestService
{
    User GetOrCreateGuest(HttpContext context);
}