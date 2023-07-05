using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContacts;
using ServiceContracts;
using Services;
using StockApp;
using StockApp.ServiceContracts;
using StockApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<StockDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<TradingOption>(builder.Configuration.GetSection("TraddingOption"));
builder.Services.AddScoped<IFinnhubRepository, FinnhubRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IFinnhubService,FinnhubService>();
builder.Services.AddScoped<IStockService, StockService>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
