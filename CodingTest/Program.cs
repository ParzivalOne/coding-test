using CodingTest.Domain.Constants;
using CodingTest.Domain.Interfaces;
using CodingTest.Domain.Models.Options;
using CodingTest.Domain.Services;
using CodingTest.Infrastructure.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

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

app.MapOpenApi();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
