using System.Data.Common;
using Bonfire.API.Middlewares;
using Bonfire.Application.Interfaces;
using Bonfire.Application.Services;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Serilog;
using Testcontainers.PostgreSql;
using Xunit;

namespace Bonfire.Tests.Extensions;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest") 
        .WithCleanUp(true)
        .Build();
    
    private AppDbContext _appDbContext = null!;
    
    private Respawner _respawner = null!;
    
    private DbConnection _connection = null!;
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _appDbContext = Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        _connection = _appDbContext.Database.GetDbConnection();
        await _connection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
        }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_connection);
    }
    
    public new async Task DisposeAsync()
    {
        await _connection.CloseAsync();
        await _container.DisposeAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
                services.RemoveDbContext<AppDbContext>();
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(_container.GetConnectionString());
                });
                services.EnsureDbCreated<AppDbContext>();
        });
    }
    
}

public static class TestsExtensions
{
    public static async void ClearDataBase(DbConnection dbConnection)
    {
        await Respawner.CreateAsync(dbConnection, new RespawnerOptions   
        {                                                                            
            DbAdapter = DbAdapter.Postgres,                                          
            SchemasToInclude = ["public"]                                            
        });                                                                          
    }
}

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }
    
    public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<T>();
        context.Database.EnsureCreated();
    }
}