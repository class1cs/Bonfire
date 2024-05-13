using Bonfire.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Persistance;

public sealed  class AppDbContext : DbContext
{ 
    public DbSet<User> Users { get; init; }
    
    public DbSet<DirectChat> DirectChats { get; init; }
        
    public DbSet<Message> Messages { get; init; }
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
}