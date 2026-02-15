using Testing3;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IGuestService, GuestRepository>();

builder.Services.AddScoped<ILikeRepository, LikeRepositoryMock>();

var app = builder.Build();


app.Urls.Add("http://0.0.0.0:3002");

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();




