using Bonfire.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Persistance;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

    public DbSet<User> Users { get; set; }

    public DbSet<Conversation> Conversations { get; set; }

    public DbSet<Message> Messages { get; set; }
    
    public DbSet<FriendRequest> FriendRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Conversation>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();;

        modelBuilder.Entity<Message>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Friends)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserFriends"));
        
        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasOne(fr => fr.Sender)
                .WithMany(u => u.FriendRequests)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict); 
            
            entity.HasOne(fr => fr.Receiver)
                .WithMany() 
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}