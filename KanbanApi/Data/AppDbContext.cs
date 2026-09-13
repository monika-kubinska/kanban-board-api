using KanbanApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<WipLimit> WipLimits => Set<WipLimit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .Property(item => item.State)
            .HasConversion(
                state => state.ToStorageValue(),
                value => ItemStateExtensions.FromStorageValue(value));

        modelBuilder.Entity<Team>().HasData(
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Zespół Alfa" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Zespół Beta" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Zespół Gamma" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Zespół Delta" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Zespół Epsilon" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "Zespół Zeta" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "Zespół Eta" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Name = "Zespół Theta" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), Name = "Zespół Iota" },
            new Team { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), Name = "Zespół Kappa" });

        modelBuilder.Entity<TeamMember>()
            .HasKey(tm => new { tm.UserId, tm.TeamId });

        modelBuilder.Entity<Board>()
            .HasOne(board => board.Team)
            .WithOne(team => team.Board)
            .HasForeignKey<Board>(board => board.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
  
