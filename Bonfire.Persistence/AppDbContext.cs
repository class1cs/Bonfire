using Bonfire.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Persistance;

public class AppDbContext : DbContext
{ 
    public DbSet<User> Users { get; set; }
    
    public DbSet<Conversation> Conversations { get; set; }
        
    public DbSet<Message> Messages { get; set; }
    
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }
    
}