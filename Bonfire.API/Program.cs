using System.Text;
using Bonfire.Abstractions;
using Bonfire.API.Middlewares;
using Bonfire.Application.Services;
using Bonfire.Persistance;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);



var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
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
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionMiddleWare>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IConversationsService, ConversationsService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = AuthOptions.Audience,
            
            ValidateLifetime = true,
            
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connString);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.Run();
