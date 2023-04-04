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

        var floatArrayConverter = new ValueConverter<float[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => float.Parse(val)).ToArray());

        var floatArrayComparer = new ValueComparer<float[]>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray());

        modelBuilder.Entity<SeasonEventRoundRaceHeatResult>()
            .Property(heatResult => heatResult.Bonuses)
            .HasConversion(floatArrayConverter)
            .Metadata
            .SetValueComparer(floatArrayComparer);

        modelBuilder
            .Entity<SeasonEventScoreRules>()
            .Property(sesr => sesr.AvailableBonuses)
            .HasConversion(floatArrayConverter)
            .Metadata
            .SetValueComparer(floatArrayComparer);

        modelBuilder
            .Entity<SeasonEventRoundRaceHeatResult>()
            .Property(heatResult => heatResult.SectorTimes)
            .HasConversion(floatArrayConverter)
            .Metadata
            .SetValueComparer(floatArrayComparer);
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
    public DbSet<SeasonEventRound> SeasonEventRounds => Set<SeasonEventRound>();
    public DbSet<SecondChanceRules> SecondChanceRules => Set<SecondChanceRules>();
    public DbSet<SeasonEventRoundRace> SeasonEventRoundRaces => Set<SeasonEventRoundRace>();
    public DbSet<SeasonEventRoundRaceResult> SeasonEventRoundRaceResults => Set<SeasonEventRoundRaceResult>();
    public DbSet<SeasonEventRoundRaceHeat> SeasonEventRoundRaceHeats => Set<SeasonEventRoundRaceHeat>();
    public DbSet<SeasonEventRoundRaceHeatResult> SeasonEventRoundRaceHeatResults => Set<SeasonEventRoundRaceHeatResult>();

}