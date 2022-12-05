using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using RedisSearchProduct.Configuration;
using RedisSearchProduct.Data.Products.Services;
using RedisSearchProduct.Data.Redis;
using RedisSearchProduct.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(policy => policy.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<RedisOptions>(options => builder.Configuration.GetSection("Redis").Bind(options));

builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<ISeedService, SeedService>();
builder.Services.AddSingleton<ISearchService, SearchService>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseSeedRoutes();
app.UseProductRoutes();

app.Run();