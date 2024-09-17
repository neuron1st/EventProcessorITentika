using Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace Context;

public class ProcessorDbContext : DbContext
{
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<Event> Events { get; set; }

    public ProcessorDbContext(DbContextOptions<ProcessorDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
