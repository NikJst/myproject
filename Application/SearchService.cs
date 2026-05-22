using Microsoft.EntityFrameworkCore;
using Testing3.DTO;
using LinqKit;
using System.Linq.Expressions;
namespace Testing3.Application;

public interface ISearch
{
    Task<List<ViewPostsDto>> SearchToWords(string query);
    Task<List<UserFilterTarget>> SearchUsers<T>(RangeFilter<T> range, UserFilterTarget Values) where T : struct, IComparable;
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
    //===========>
    public async Task<List<UserFilterTarget>> SearchUsers<T>(RangeFilter<T> range, UserFilterTarget filterTarget) where T : struct, IComparable
    {
        IQueryable<User> result = _dbcontext.Users;
        var param = Expression.Parameter(typeof(User), "u");
        // var predicate = PredicateBuilder.New<User>(true);

        Expression filterBody = Expression.Constant(true);


        if (filterTarget.Age is not null && range.Min is not null && range.Max is not null)
        {
            var propertyDatetime = Expression.Property(param, nameof(User.Age));
            var constantMin = Expression.Constant(range.Min, propertyDatetime.Type);
            var constantMax = Expression.Constant(range.Max, propertyDatetime.Type);
            var leftAge = Expression.GreaterThanOrEqual(propertyDatetime, constantMin);
            var rightAge = Expression.LessThanOrEqual(propertyDatetime, constantMax); //=====> изменить на нужный тип сравнения
            var newcomparison = Expression.AndAlso(leftAge, rightAge);
            filterBody = Expression.AndAlso(filterBody, newcomparison);
            // result = result.Where(u => u.CreatedAt >= dateFilter.Min && u.CreatedAt <= dateFilter.Max);
            _logger.LogWarning("Age filter");

        }
        if (!string.IsNullOrWhiteSpace(filterTarget.Username))
        {
            var propName = Expression.Property(param, nameof(User.Username));
            var constName = Expression.Constant(filterTarget.Username, propName.Type); //изменил с value
            var newComparison = Expression.Equal(propName, constName);
            filterBody = Expression.AndAlso(filterBody, newComparison);
            _logger.LogWarning("Name filter");
        }
        if (!string.IsNullOrWhiteSpace(filterTarget.City))
        {
            var propCity = Expression.Property(param, nameof(User.City));
            var constCity = Expression.Constant(filterTarget.City, propCity.Type);
            var newComparison = Expression.Equal(propCity, constCity);
            filterBody = Expression.AndAlso(filterBody, newComparison);
            _logger.LogWarning("City filter");
        }
        if (!string.IsNullOrWhiteSpace(filterTarget.Hobby))
        {
            var propHobby = Expression.Property(param, nameof(User.Hobby));
            var constHobby = Expression.Constant(filterTarget.Hobby, propHobby.Type);
            var newComparison = Expression.Equal(propHobby, constHobby);
            filterBody = Expression.AndAlso(filterBody, newComparison);
            _logger.LogWarning("Hobby filter");
        }


        if (filterTarget.PostCount is not null)
        {
            var propPostCount = Expression.Property(param, nameof(User.PostCount));
            var constPostCount = Expression.Constant(filterTarget.PostCount);
            var leftPostCount = Expression.GreaterThanOrEqual(propPostCount, constPostCount);
            var rightPostCount = Expression.LessThanOrEqual(propPostCount, constPostCount);
            var newComparison = Expression.AndAlso(leftPostCount, rightPostCount);
            filterBody = Expression.AndAlso(filterBody, newComparison);
            _logger.LogWarning("PostCount filter");
        }

        var lambda = Expression.Lambda<Func<User, bool>>(filterBody, param);
        Console.WriteLine($"Lambda ==> {lambda}");
        result = result.Where(lambda);

        return await result.Select(u => new UserFilterTarget
        {
            Username = u.Username ?? string.Empty,
            Age = u.Age,
            Date = u.CreatedAt,
            PostCount = u.PostCount,
            City = u.City,
            Hobby = u.Hobby,
        }).ToListAsync();
    }
}


// case UserFilterTarget.RegistrationDate:
//     _logger.LogWarning("RegistrationDate filter");
//     // Компилятор должен быть уверен, что T — это DateTime
//     if (range is RangeFilter<DateTime> dateFilter)
//     {
//         var propertyDatetime = Expression.Property(param, nameof(User.CreatedAt));
//         var typedValueMin = Convert.ChangeType(range.Min, propertyDatetime.Type);
//         var typedValueMax = Convert.ChangeType(range.Max, propertyDatetime.Type);
//         var constantMin = Expression.Constant(typedValueMin, propertyDatetime.Type);
//         var constantMax = Expression.Constant(typedValueMax, propertyDatetime.Type);

//         var liftcomparison = Expression.GreaterThanOrEqual(propertyDatetime, constantMin);
//         var rightcomparison = Expression.LessThanOrEqual(propertyDatetime, constantMax);
//         comparison = Expression.AndAlso(liftcomparison, rightcomparison);
//         // result = result.Where(u => u.CreatedAt >= dateFilter.Min && u.CreatedAt <= dateFilter.Max);
//     }
//     break;

