using System.Security.Claims;
using JustEmpire.DbContexts;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Threading.RateLimiting;
using JustEmpire.Services.Classes.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
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

var rankRepository = new RankRepository();

builder.Services.AddMvc();
builder.Services.AddDbContext<JustEmpireDbContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ArticleRepository>();
builder.Services.AddScoped<ServiceRepository>();
builder.Services.AddScoped<ServiceCategoryRepository>();
builder.Services.AddScoped<ServiceVersionRepository>();
builder.Services.AddScoped<ServiceImageRepository>();
builder.Services.AddScoped<PageViewRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton(rankRepository);
builder.Services.AddScoped<UserAccessor>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:JwtEncryptionKey").Value)),
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

builder.Services.AddAuthorization(options =>
{
    // Add policy for users who have permission to manage approvements
    options.AddPolicy("CanManageApprovements", policyBuilder =>
    {
        policyBuilder.RequireAssertion(context =>
        {
            var rankId = context.User.Claims.FirstOrDefault(c => c.Type == "RankId").Value;
            bool hasPermissionToManageApprovements = rankRepository.GetById(int.Parse(rankId)).ManageApprovements;
            return hasPermissionToManageApprovements;
        }); 
    });
});

// Configuring rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("client", options =>
    {
        options.Window = TimeSpan.FromSeconds(30);
        options.PermitLimit = 50;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("auth", options =>
    {
        options.Window = TimeSpan.FromSeconds(60);
        options.PermitLimit = 4;
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