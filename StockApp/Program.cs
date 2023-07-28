using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository;
using RepositoryContacts;
using Serilog;
using ServiceContracts;
using Services;
using StockApp;
using StockApp.Filters.ActionFilters;
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

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>() // tao bang trong co so du lieu
    .AddEntityFrameworkStores<StockDbContext>() // context su dung trong app
    .AddDefaultTokenProviders()// usiing setup new password,change email
    // create repository cua user va role de thao tac du lieu nguoi dung trong dbcontext
    .AddUserStore<UserStore<ApplicationUser,ApplicationRole,StockDbContext,Guid>>() // cho user
    .AddRoleStore<RoleStore<ApplicationRole, StockDbContext, Guid>>(); // cho role

builder.Services.AddTransient<CreateOrderActionFilter>();


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
