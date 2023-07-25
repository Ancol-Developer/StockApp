using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContacts;
using Serilog;
using ServiceContracts;
using Services;
using StockApp;
using StockApp.ServiceContracts;
using StockApp.Services;

var builder = WebApplication.CreateBuilder(args);
// Serilog 
builder.Host.UseSerilog((HostBuilderContext context,IServiceProvider service,LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) // read configuration settings from built-in Iconfiguration
    .ReadFrom.Services(service); // read out current app's services and make them avaliable to serilog
});

//Service
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
