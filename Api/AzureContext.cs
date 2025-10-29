namespace Api;

using BlazorApp.Shared;
using Microsoft.EntityFrameworkCore;

public class AzureContext : DbContext
{

    public AzureContext(DbContextOptions<AzureContext> options) : base(options)
    {
    }
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseCosmos(
            Environment.GetEnvironmentVariable("CosmosDbUri"),
            Environment.GetEnvironmentVariable("AzureKey"),
            "BBDB"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>().ToContainer("Messages").HasPartitionKey(m => m.UserName).HasNoDiscriminator();
    }
}