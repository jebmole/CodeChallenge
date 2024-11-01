using Ease.CodeChallenge.Application.Interfaces;
using Ease.CodeChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ease.CodeChallenge.Infrastructure.Persistence.DbContexts;

public partial class MetadataDbContext : DbContext, IApplicationContext
{
    public MetadataDbContext()
    {
    }

    public MetadataDbContext(DbContextOptions<MetadataDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserMetadata> UserMetadata { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
       
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
