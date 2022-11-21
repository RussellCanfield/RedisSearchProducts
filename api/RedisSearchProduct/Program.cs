using RedisSearchProduct.Configuration;
using RedisSearchProduct.Data.Products.Services;
using RedisSearchProduct.Data.Redis;
using RedisSearchProduct.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RedisOptions>(options => builder.Configuration.GetSection("Redis").Bind(options));

builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<ISeedService, SeedService>();
builder.Services.AddSingleton<ISearchService, SearchService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSeedRoutes();
app.UseProductRoutes();

app.Run();