var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(
    $"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<
    CitiesRpcServer.Services.GameService>();
builder.Services.AddSingleton<
    CitiesRpcServer.Services.LobbyService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();