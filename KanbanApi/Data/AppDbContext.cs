using KanbanApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<WipLimit> WipLimits => Set<WipLimit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamMember>()
            .HasKey(tm => new { tm.UserId, tm.TeamId });
    }
}
  
