using System.Text;
using Bonfire.API.Middlewares;
using Bonfire.Application.Hubs;
using Bonfire.Application.Interfaces;
using Bonfire.Application.Services;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
var issuer = builder.Configuration.GetValue<string>("AuthOptions:Issuer");
var audience = builder.Configuration.GetValue<string>("AuthOptions:Audience");
var key = builder.Configuration.GetValue<string>("AuthOptions:Key");
var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
var frontendUrl = builder.Configuration.GetValue<string>("FrontendUrl") ?? "http://localhost:3000";

var services = builder.Services;

// Добавляем сервисы
services.AddEndpointsApiExplorer();

// Настройка Swagger
services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new()
    {
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Авторизация через JWT."
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Настройка контроллеров и валидации
services.AddProblemDetails()
    .AddHttpContextAccessor()
    .AddSerilog()
    .AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = c =>
        {
            var errors =
                c.ModelState
                    .Where(x => x.Value?.Errors.Any() == true)
                    .SelectMany(x => x.Value.Errors.Select(e => new ValidationFailure(x.Key, e.ErrorMessage)))
                    .ToArray();

            throw new ValidationException("invalid data in request", errors);
        };
    });

// Регистрация сервисов
services.AddExceptionHandler<ExceptionMiddleware>()
    .AddScoped<ITokenService, TokenService>()
    .AddScoped<IIdentityService, IdentityService>()
    .AddScoped<IConversationsService, ConversationsService>()
    .AddScoped<IMessagesService, MessagesService>()
    .AddScoped<IUserInfoService, UserInfoService>()
    .AddScoped<IUserService, UserService>()
    .AddSingleton(TimeProvider.System)
    .AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connString);
    });

services.AddValidatorsFromAssemblyContaining<Program>();
services.AddFluentValidationAutoValidation();

// Настройка логирования
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// Настройка CORS
services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Важно для JWT и SignalR
    });
});

// Настройка аутентификации
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            IssuerSigningKey = symmetricSecurityKey,
            ValidateIssuerSigningKey = true
        };
        
        // Для SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/bonfireHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

services.AddAuthorization();
services.AddSignalR();

var app = builder.Build();

// Настройка middleware pipeline (важен порядок!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();
app.MapHub<BonfireHub>("/bonfireHub");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.Run();

public partial class Program { }