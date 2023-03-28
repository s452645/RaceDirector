using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        var intArrayConverter = new ValueConverter<int[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val)).ToArray());

        var intArrayComparer = new ValueComparer<int[]>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray());

        modelBuilder.Entity<SeasonEventScoreRules>()
            .Property(sesr => sesr.AvailableBonuses)
            .HasConversion(intArrayConverter);

        modelBuilder
            .Entity<SeasonEventScoreRules>()
            .Property(sesr => sesr.AvailableBonuses)
            .Metadata
            .SetValueComparer(intArrayComparer);

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
    public DbSet<BreakBeamSensor> BreakBeamSensors => Set<BreakBeamSensor>();
    public DbSet<Checkpoint> Checkpoints => Set<Checkpoint>();
    public DbSet<Circuit> Circuits => Set<Circuit>();
    public DbSet<PicoBoard> PicoBoards => Set<PicoBoard>();
    public DbSet<SeasonEvent> SeasonEvents => Set<SeasonEvent>();
    public DbSet<SeasonEventScoreRules> SeasonEventScoreRules => Set<SeasonEventScoreRules>();
}