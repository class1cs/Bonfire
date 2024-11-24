using System.Text;
using Bonfire.API.Middlewares;
using Bonfire.Application.Interfaces;
using Bonfire.Application.Services;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");

var issuer = builder.Configuration.GetValue<string>("AuthOptions:Issuer");
var audience = builder.Configuration.GetValue<string>("AuthOptions:Audience");
var key = builder.Configuration.GetValue<string>("AuthOptions:Key");
var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http, In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Авторизация через JWT."
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
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

services.AddProblemDetails()
    .AddHttpContextAccessor()
    .AddControllers();  

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


services.AddAuthorization();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            IssuerSigningKey = symmetricSecurityKey,
            ValidateIssuerSigningKey = true
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();

app.Run();