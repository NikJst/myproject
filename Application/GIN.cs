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
                ORDER BY ""CreatedAt"" DESC --функция ts_rank. Сначала самые подходящие. 
                -- Здесь вы используете возможности Postgres, которые невозможно адекватно выразить через чистый LINQ")
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
        //===============>
        // // 1. Создаем текст команды с "заглушкой"
        // string sql = "SELECT * FROM users WHERE login = @login";
        // var command = new SqlCommand(sql, connection);

        // // 2. Явно передаем значение отдельно
        // command.Parameters.AddWithValue("@login", userInput); 

        // // 3. Выполняем
        // command.ExecuteReader();




        //         using System.Data.SqlClient;

        // // 1. Предположим, это данные от пользователя (очень опасные!)
        // string dangerousInput = "' OR 1=1 --"; 

        // // 2. Пишем SQL-запрос. 
        // // Вместо вставки переменной пишем ПЛЕЙСХОЛДЕР (заглушку) — @login
        // string sql = "SELECT * FROM Users WHERE Login = @login";

        // using (SqlConnection connection = new SqlConnection(connectionString))
        // {
        //     SqlCommand command = new SqlCommand(sql, connection);

        //     // 3. ВОТ ОНО: Передача через Parameters.
        //     // Мы говорим: "Вместо @login вставь значение из переменной dangerousInput"
        //     command.Parameters.AddWithValue("@login", dangerousInput);

        //     connection.Open();
        //     using (SqlDataReader reader = command.ExecuteReader())
        //     {
        //         // База данных получит запрос и данные РАЗДЕЛЬНО.
        //         // Она будет искать пользователя, у которого логин буквально равен 
        //         // строке "' OR 1=1 --", и никого не найдет. Атака провалилась.
        //     }
        // }


    }
}