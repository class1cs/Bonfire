using Bonfire.API.Middlewares;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<ExceptionMiddleWare>();
builder.Services.AddAutoMapper(typeof(AppMapProfile));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySQL("server=localhost;user=root;database=bonfire;password=admin");
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

app.Run();
