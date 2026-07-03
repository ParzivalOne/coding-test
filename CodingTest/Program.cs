using CodingTest.Domain.Constants;
using CodingTest.Domain.Interfaces;
using CodingTest.Domain.Models.Options;
using CodingTest.Domain.Services;
using CodingTest.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CodingTest API",
        Version = "v1",
        Description = "API to retrieve best Hacker News stories"
    });
});

builder.Services.Configure<HackerNewsApiOptions>(builder.Configuration.GetSection("HackerNewsApi"));
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));

builder.Services.AddHttpClient(HttpClientConstants.HACKER_NEWS_CLIENT_NAME, (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<HackerNewsApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services
    .AddMemoryCache()
    .AddSingleton<INewsService, NewsService>()
    .AddSingleton<IHackerNewsApiService, HackerNewsApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CodingTest API V1");
    // Serve the UI at application root
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
