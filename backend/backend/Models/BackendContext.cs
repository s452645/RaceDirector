using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backenend.Models;

public class BackendContext : DbContext
{
    public BackendContext(DbContextOptions<BackendContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Photo>()
            .Property(x => x.UploadDate)
            .HasDefaultValueSql("getutcdate()");
    }   

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<CarSize> CarSizes => Set<CarSize>();
    public DbSet<OfficialName> OfficialNames => Set<OfficialName>();
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Pot> Pots => Set<Pot>();
    public DbSet<Series> Series => Set<Series>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<UnitValue> UnitValues => Set<UnitValue>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<SeasonCarStanding> SeasonCarStandings => Set<SeasonCarStanding>();
    public DbSet<SeasonTeamStanding> SeasonTeamStandings => Set<SeasonTeamStanding>();
}