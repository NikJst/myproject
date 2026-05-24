using Microsoft.EntityFrameworkCore;
using Testing3.DTO;
using LinqKit;
using System.Linq.Expressions;
namespace Testing3.Application;

public interface ISearchService
{
    Task<List<ViewPostDto>> SearchToWords(string query);
    Task<List<UserFilterTarget>> SearchUsers(UserFilterTarget Values);
}


public class SearchService : ISearchService
{

    private readonly ApplicationDbContext _dbcontext;
    private readonly ILogger<SearchService> _logger;
    public SearchService(ApplicationDbContext dbcontext, ILogger<SearchService> logger)
    {
        _dbcontext = dbcontext;
        _logger = logger;
    }
    public async Task<List<ViewPostDto>> SearchToWords(string query)
    {

        var results = await _dbcontext.Posts.FromSql($@"
                SELECT ""Id"", ""Text"", ""UserId"", ""CreatedAt""
                FROM ""Posts"" 
                WHERE to_tsvector('russian', ""Text"") @@ websearch_to_tsquery('russian', {query})
                ORDER BY ""CreatedAt"" DESC")
            .Include(p => p.Likes)
            .Select(p => new ViewPostDto
            {
                PostId = p.Id,
                UserId = p.UserId,
                Text = p.Text,
                IsOnline = true //временно
            })
            .ToListAsync();

        return results;

    }

    public async Task<List<UserFilterTarget>> SearchUsers(UserFilterTarget filterTarget)
    {
        IQueryable<User> result = _dbcontext.Users;
        var param = Expression.Parameter(typeof(User), "u");
        // var predicate = PredicateBuilder.New<User>(true);

        Expression filterBody = Expression.Constant(true);


        if (filterTarget.Age is not null)
        {
            var propAge = Expression.Property(param, nameof(User.Age));
            var constMin = Expression.Constant(filterTarget.Age.Min);
            var constMax = Expression.Constant(filterTarget.Age.Max);
            var minExpression = Expression.GreaterThanOrEqual(propAge, constMin);
            var maxExpression = Expression.LessThanOrEqual(propAge, constMax);
            var finalExpression = Expression.AndAlso(minExpression, maxExpression);
            filterBody = Expression.AndAlso(filterBody, finalExpression);
            // result = result.Where(u => u.CreatedAt >= dateFilter.Min && u.CreatedAt <= dateFilter.Max);
            _logger.LogWarning("Age filter");
            System.Console.WriteLine("Age filter ==> " + filterBody);

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
            var varMin = Convert.ChangeType(filterTarget.PostCount.Min, propPostCount.Type);
            var varMax = Convert.ChangeType(filterTarget.PostCount.Max, propPostCount.Type);
            var constMin = Expression.Constant(varMin, propPostCount.Type);
            var constMax = Expression.Constant(varMax, propPostCount.Type);
            var leftVar = Expression.GreaterThanOrEqual(propPostCount, constMin);
            var rightVar = Expression.LessThanOrEqual(propPostCount, constMax);
            var newComparison = Expression.AndAlso(leftVar, rightVar);
            filterBody = Expression.AndAlso(filterBody, newComparison);
            _logger.LogWarning("PostCount filter");
        }
        if (filterTarget.Date is not null)
        {
            var propDate = Expression.Property(param, nameof(User.CreatedAt));
            var varMin = Convert.ChangeType(filterTarget.Date.Min, propDate.Type);
            var varMax = Convert.ChangeType(filterTarget.Date.Max, propDate.Type);

            var constantMin = Expression.Constant(varMin, propDate.Type);
            var constantMax = Expression.Constant(varMax, propDate.Type);

            var leftComparison = Expression.GreaterThanOrEqual(propDate, constantMin);
            var rightComparison = Expression.LessThanOrEqual(propDate, constantMax);
            var newComparison = Expression.AndAlso(leftComparison, rightComparison);
            filterBody = Expression.AndAlso(filterBody, newComparison);
            // result = result.Where(u => u.CreatedAt >= dateFilter.Min && u.CreatedAt <= dateFilter.Max);
        }


        var lambda = Expression.Lambda<Func<User, bool>>(filterBody, param);
        Console.WriteLine($"Lambda ==> {lambda}");
        result = result.Where(lambda);

        return await result.Select(u => new UserFilterTarget
        {
            Username = u.Username ?? string.Empty,
            AgeOut = u.Age,
            DateOut = u.CreatedAt,
            PostCountOut = u.PostCount,
            City = u.City,
            Hobby = u.Hobby,
        }).ToListAsync();
    }
}

// Компилятор должен быть уверен, что T — это DateTime

