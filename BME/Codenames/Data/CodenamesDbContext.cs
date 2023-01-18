using Codenames.Model;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Codenames.Data;

public partial class CodenamesDbContext : ApiAuthorizationDbContext<User>
{
    public DbSet<User> DbUsers { get; set; } = null!;
    public DbSet<Field> Fields { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Word> Words { get; set; } = null!;
    public CodenamesDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog = Codenames; Integrated Security= True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
