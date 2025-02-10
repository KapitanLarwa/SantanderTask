using HackerNews.BusinessLogic.Services;
using HackerNews.Domain.Interfaces;
using HackerNews.Infrastructure.Caching;
using HackerNews.Infrastructure.Clients;
using Microsoft.OpenApi.Models;
using Unity;

var builder = WebApplication.CreateBuilder(args);
var container = new UnityContainer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HackerNews API", Version = "v1" });
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IHackerNewsClient, HackerNewsClient>();
builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNews API v1"));
}

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.Run();
