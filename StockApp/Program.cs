using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
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

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredUniqueChars = 3; // eg: AB12AB (unique characters are A,B,1,2)
}) // tao bang trong co so du lieu
    .AddEntityFrameworkStores<StockDbContext>() // context su dung trong app
    .AddDefaultTokenProviders()// usiing setup new password,change email
    // create repository cua user va role de thao tac du lieu nguoi dung trong dbcontext
    .AddUserStore<UserStore<ApplicationUser,ApplicationRole,StockDbContext,Guid>>() // cho user
    .AddRoleStore<RoleStore<ApplicationRole, StockDbContext, Guid>>(); // cho role
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy= new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    // enfores authoriation policy (user must be authenticated) for all the action methods
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

//Filter
builder.Services.AddTransient<CreateOrderActionFilter>();


builder.Services.Configure<TradingOption>(builder.Configuration.GetSection("TraddingOption"));
builder.Services.AddScoped<IFinnhubRepository, FinnhubRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IFinnhubService,FinnhubService>();
builder.Services.AddScoped<IStockService, StockService>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();// identity action method based route
app.UseAuthentication(); // erading identity cookie
app.UseAuthorization();// validate access permissions of the user
app.MapControllers();// execute the filter pipeline(action+filters)

app.Run();
