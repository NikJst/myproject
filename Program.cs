using Testing3;
using Testing3.Application;
using Serilog;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

builder.Services.AddControllers();
// Enable middleware to serve generated Swagger as a JSON endpoint.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IOnlineService, OnlineService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ITest, Test>();


builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Urls.Add("http://0.0.0.0:3000");

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapControllers();

//добавиь отдельный middlewere для проверки пользователя на авторизацию

app.Run();




