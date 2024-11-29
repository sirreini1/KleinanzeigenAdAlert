using KleinanzeigenAdAlert.DB.entities;
using Microsoft.EntityFrameworkCore;

namespace KleinanzeigenAdAlert.DB;

public class AppDbContext : DbContext
{
    public DbSet<FlatAdEntity> FlatAds { get; set; }
    public DbSet<UserChatIdPair> UserChatIdPairs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Environment.GetEnvironmentVariable("DB_PATH") ?? "database.db";
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}