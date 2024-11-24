using Bonfire.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Persistance;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Conversation> Conversations { get; set; }

    public DbSet<Message> Messages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Conversation>()
            .Property(p => p.Id);
         modelBuilder.Entity<Message>()
             .Property(p => p.Id)
             .ValueGeneratedOnAdd();
    }
    
}