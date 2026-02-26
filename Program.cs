using Testing3;
using Microsoft.OpenApi;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();

// Enable middleware to serve generated Swagger as a JSON endpoint.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<ILikePost, LikePost>();
builder.Services.AddSingleton<ILikeRepository, LikeRepositoryMock>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddSingleton<IPostService, PostService>();
builder.Services.AddSingleton<IPostRepository, PostRepository>();// здесь важно чтобы репозитории жил все время жизни приложения, а не каждый запрос
builder.Services.AddSingleton<IUserAndGuestRepository, UserAndGuestRepository>();

// builder.Services.AddScoped<ILikeRepository, LikeRepositoryMock>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Urls.Add("http://0.0.0.0:3000");

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapControllers();

app.Run();




