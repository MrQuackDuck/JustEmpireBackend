using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Enums;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Replace with your Angular app's URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Setting up logging
var logPath = builder.Configuration.GetValue<string>("Logging:FilePath").Replace("log.txt", $"{DateTime.Now.ToString("yyyy-MM-dd [hh.mm.ss]")}.txt");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(logPath)
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddMvc();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ArticleRepository>();
builder.Services.AddScoped<ServiceRepository>();
builder.Services.AddScoped<ServiceCategoryRepository>();
builder.Services.AddScoped<ServiceVersionRepository>();
builder.Services.AddScoped<ServiceImageRepository>();
builder.Services.AddScoped<PageViewRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RankRepository>();
builder.Services.AddScoped<UserAccessor>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Configure the cookie name
                context.Token = context.Request.Cookies["jwt"];

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:5228").AllowAnyMethod().AllowAnyHeader();
    }));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("client", options =>
    {
        options.Window = TimeSpan.FromSeconds(20);
        options.PermitLimit = 20;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

var app = builder.Build();

app.UseCors();

app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "API/{controller}/{action}/{serviceId?}");

app.Run();