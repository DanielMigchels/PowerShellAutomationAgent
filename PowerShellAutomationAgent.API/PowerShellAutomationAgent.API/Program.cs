using PowerShellAutomationAgent.API.Data;
using PowerShellAutomationAgent.API.Options;
using PowerShellAutomationAgent.API.Services.Authentication;
using PowerShellAutomationAgent.API.Services.BackgroundJob;
using PowerShellAutomationAgent.API.Services.Dashboard;
using PowerShellAutomationAgent.API.Services.Jobs;
using PowerShellAutomationAgent.API.Services.Projects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using System.Text;

string LoggingFilePath = @"C:\ProgramData\PowerShellAutomationAgent\logs\app.log";

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(LoggingFilePath, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {SourceContext} - {Message} ({Caller}){NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddControllers();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot/powershellautomationagent.ui/browser/";
});

builder.Services.AddWindowsService();

builder.Services.AddOptions<JwtOptions>().Bind(builder.Configuration.GetSection("JwtOptions")).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<AccessOptions>().Bind(builder.Configuration.GetSection("Access")).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IDashboardService, DashboardService>();
builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddTransient<IJobService, JobService>(); 
builder.Services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();
builder.Services.AddHostedService<JobWorkerService>();
builder.Services.AddScoped<JobExecutorService>();

var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:Secret"] ?? throw new Exception("Could not read JWT secret"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PowerShellAutomationAgent.API", Version = "v1" });
    c.AddServer(new OpenApiServer { Url = "" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter a valid token in the format 'Bearer {token}'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSpaStaticFiles();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSpa(spa =>
{
    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    }
});

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
await db.Database.MigrateAsync();

app.UseSerilogRequestLogging();

app.MapControllers();

try
{
    Log.Information("Starting the web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
