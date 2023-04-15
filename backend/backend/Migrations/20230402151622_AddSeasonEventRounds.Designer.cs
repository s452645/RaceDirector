﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using backend.Models;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(BackendContext))]
    [Migration("20230402151622_AddSeasonEventRounds")]
    partial class AddSeasonEventRounds
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("backend.Models.BreakBeamSensor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BoardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Pin")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("BreakBeamSensors");
                });

            modelBuilder.Entity("backend.Models.Car", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MainPhotoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MainPhotoId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("backend.Models.CarSize", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("HeightId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("LengthId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("WeightId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("WidthId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CarId")
                        .IsUnique();

                    b.HasIndex("HeightId");

                    b.HasIndex("LengthId");

                    b.HasIndex("WeightId");

                    b.HasIndex("WidthId");

                    b.ToTable("CarSizes");
                });

            modelBuilder.Entity("backend.Models.Checkpoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BreakBeamSensorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CircuitId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BreakBeamSensorId");

                    b.HasIndex("CircuitId");

                    b.ToTable("Checkpoints");
                });

            modelBuilder.Entity("backend.Models.Circuit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Circuits");
                });

            modelBuilder.Entity("backend.Models.OfficialName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModelName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SeriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("WikiLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CarId")
                        .IsUnique();

                    b.HasIndex("SeriesId");

                    b.ToTable("OfficialNames");
                });

            modelBuilder.Entity("backend.Models.Owner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PhotoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PhotoId");

                    b.ToTable("Owners");
                });

            modelBuilder.Entity("backend.Models.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("File")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("Height")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UploadDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<int?>("Width")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("backend.Models.PicoBoard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("IPAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PicoBoards");
                });

            modelBuilder.Entity("backend.Models.Pot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Hierarchy")
                        .HasColumnType("int");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Pots");
                });

            modelBuilder.Entity("backend.Models.Season", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("backend.Models.SeasonCarStanding", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Place")
                        .HasColumnType("int");

                    b.Property<double>("Points")
                        .HasColumnType("float");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.HasIndex("SeasonId");

                    b.ToTable("SeasonCarStandings");
                });

            modelBuilder.Entity("backend.Models.SeasonEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CircuitId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ScoreRulesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CircuitId")
                        .IsUnique()
                        .HasFilter("[CircuitId] IS NOT NULL");

                    b.HasIndex("ScoreRulesId")
                        .IsUnique()
                        .HasFilter("[ScoreRulesId] IS NOT NULL");

                    b.HasIndex("SeasonId");

                    b.ToTable("SeasonEvents");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRound", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DroppedCarsPointsStrategy")
                        .HasColumnType("int");

                    b.Property<int>("DroppedCarsPositionDefinementStrategy")
                        .HasColumnType("int");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("PointsStrategy")
                        .HasColumnType("int");

                    b.Property<Guid>("SeasonEventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SecondChanceRulesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SeasonEventId");

                    b.HasIndex("SecondChanceRulesId")
                        .IsUnique();

                    b.ToTable("SeasonEventRounds");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRace", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("InstantAdvancements")
                        .HasColumnType("int");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<Guid?>("SeasonEventRoundId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SecondChances")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SeasonEventRoundId");

                    b.ToTable("SeasonEventRoundRaces");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRaceHeat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RaceId");

                    b.ToTable("SeasonEventRoundRaceHeats");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRaceHeatResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("AdvantagePoints")
                        .HasColumnType("real");

                    b.Property<string>("Bonuses")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("DistancePoints")
                        .HasColumnType("real");

                    b.Property<float>("FullTime")
                        .HasColumnType("real");

                    b.Property<float>("PointsSummed")
                        .HasColumnType("real");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<Guid?>("SeasonEventRoundRaceHeatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SectorTimes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("TimePoints")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.HasIndex("SeasonEventRoundRaceHeatId");

                    b.ToTable("SeasonEventRoundRaceHeatResults");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRaceResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("Points")
                        .HasColumnType("real");

                    b.Property<int?>("Position")
                        .HasColumnType("int");

                    b.Property<int>("RaceOutcome")
                        .HasColumnType("int");

                    b.Property<Guid>("SeasonEventRoundRaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.HasIndex("SeasonEventRoundRaceId");

                    b.ToTable("SeasonEventRoundRaceResults");
                });

            modelBuilder.Entity("backend.Models.SeasonEventScoreRules", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AvailableBonuses")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("DistanceMultiplier")
                        .HasColumnType("real");

                    b.Property<bool>("TheMoreTheBetter")
                        .HasColumnType("bit");

                    b.Property<float>("TimeMultiplier")
                        .HasColumnType("real");

                    b.Property<float>("UnfinishedSectorPenaltyPoints")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("SeasonEventScoreRules");
                });

            modelBuilder.Entity("backend.Models.SeasonTeamStanding", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Place")
                        .HasColumnType("int");

                    b.Property<double>("Points")
                        .HasColumnType("float");

                    b.Property<Guid?>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.HasIndex("TeamId");

                    b.ToTable("SeasonTeamStandings");
                });

            modelBuilder.Entity("backend.Models.SecondChanceRules", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AdvancesCount")
                        .HasColumnType("int");

                    b.Property<int>("PointsStrategy")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SecondChanceRules");
                });

            modelBuilder.Entity("backend.Models.Series", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstLevelName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecondLevelName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("backend.Models.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("backend.Models.UnitValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UnitValues");
                });

            modelBuilder.Entity("CarPot", b =>
                {
                    b.Property<Guid>("CarsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PotsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CarsId", "PotsId");

                    b.HasIndex("PotsId");

                    b.ToTable("CarPot");
                });

            modelBuilder.Entity("CarSeasonEventRound", b =>
                {
                    b.Property<Guid>("ParticipantsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SeasonEventRoundsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ParticipantsId", "SeasonEventRoundsId");

                    b.HasIndex("SeasonEventRoundsId");

                    b.ToTable("CarSeasonEventRound");
                });

            modelBuilder.Entity("backend.Models.BreakBeamSensor", b =>
                {
                    b.HasOne("backend.Models.PicoBoard", "Board")
                        .WithMany("BreakBeamSensors")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");
                });

            modelBuilder.Entity("backend.Models.Car", b =>
                {
                    b.HasOne("backend.Models.Photo", "MainPhoto")
                        .WithMany()
                        .HasForeignKey("MainPhotoId");

                    b.HasOne("backend.Models.Owner", "Owner")
                        .WithMany("Cars")
                        .HasForeignKey("OwnerId");

                    b.Navigation("MainPhoto");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("backend.Models.CarSize", b =>
                {
                    b.HasOne("backend.Models.Car", "Car")
                        .WithOne("Size")
                        .HasForeignKey("backend.Models.CarSize", "CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.UnitValue", "Height")
                        .WithMany()
                        .HasForeignKey("HeightId");

                    b.HasOne("backend.Models.UnitValue", "Length")
                        .WithMany()
                        .HasForeignKey("LengthId");

                    b.HasOne("backend.Models.UnitValue", "Weight")
                        .WithMany()
                        .HasForeignKey("WeightId");

                    b.HasOne("backend.Models.UnitValue", "Width")
                        .WithMany()
                        .HasForeignKey("WidthId");

                    b.Navigation("Car");

                    b.Navigation("Height");

                    b.Navigation("Length");

                    b.Navigation("Weight");

                    b.Navigation("Width");
                });

            modelBuilder.Entity("backend.Models.Checkpoint", b =>
                {
                    b.HasOne("backend.Models.BreakBeamSensor", "BreakBeamSensor")
                        .WithMany()
                        .HasForeignKey("BreakBeamSensorId");

                    b.HasOne("backend.Models.Circuit", "Circuit")
                        .WithMany("Checkpoints")
                        .HasForeignKey("CircuitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BreakBeamSensor");

                    b.Navigation("Circuit");
                });

            modelBuilder.Entity("backend.Models.OfficialName", b =>
                {
                    b.HasOne("backend.Models.Car", "Car")
                        .WithOne("OfficialName")
                        .HasForeignKey("backend.Models.OfficialName", "CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.Series", "Series")
                        .WithMany("OfficialNames")
                        .HasForeignKey("SeriesId");

                    b.Navigation("Car");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("backend.Models.Owner", b =>
                {
                    b.HasOne("backend.Models.Photo", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId");

                    b.Navigation("Photo");
                });

            modelBuilder.Entity("backend.Models.Photo", b =>
                {
                    b.HasOne("backend.Models.Car", null)
                        .WithMany("Photos")
                        .HasForeignKey("CarId");
                });

            modelBuilder.Entity("backend.Models.Pot", b =>
                {
                    b.HasOne("backend.Models.Team", "Team")
                        .WithMany("Pots")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("backend.Models.SeasonCarStanding", b =>
                {
                    b.HasOne("backend.Models.Car", "Car")
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.Season", "Season")
                        .WithMany("Standings")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");

                    b.Navigation("Season");
                });

            modelBuilder.Entity("backend.Models.SeasonEvent", b =>
                {
                    b.HasOne("backend.Models.Circuit", "Circuit")
                        .WithOne("SeasonEvent")
                        .HasForeignKey("backend.Models.SeasonEvent", "CircuitId");

                    b.HasOne("backend.Models.SeasonEventScoreRules", "ScoreRules")
                        .WithOne("SeasonEvent")
                        .HasForeignKey("backend.Models.SeasonEvent", "ScoreRulesId");

                    b.HasOne("backend.Models.Season", "Season")
                        .WithMany("Events")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Circuit");

                    b.Navigation("ScoreRules");

                    b.Navigation("Season");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRound", b =>
                {
                    b.HasOne("backend.Models.SeasonEvent", "SeasonEvent")
                        .WithMany("Rounds")
                        .HasForeignKey("SeasonEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.SecondChanceRules", "SecondChanceRules")
                        .WithOne("Round")
                        .HasForeignKey("backend.Models.SeasonEventRound", "SecondChanceRulesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SeasonEvent");

                    b.Navigation("SecondChanceRules");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRace", b =>
                {
                    b.HasOne("backend.Models.SeasonEventRound", null)
                        .WithMany("Races")
                        .HasForeignKey("SeasonEventRoundId");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRaceHeat", b =>
                {
                    b.HasOne("backend.Models.SeasonEventRoundRace", "Race")
                        .WithMany("Heats")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Race");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRaceHeatResult", b =>
                {
                    b.HasOne("backend.Models.Car", "Car")
                        .WithMany("HeatResults")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.SeasonEventRoundRaceHeat", null)
                        .WithMany("Results")
                        .HasForeignKey("SeasonEventRoundRaceHeatId");

                    b.Navigation("Car");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRaceResult", b =>
                {
                    b.HasOne("backend.Models.Car", "Car")
                        .WithMany("RaceResults")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.SeasonEventRoundRace", "SeasonEventRoundRace")
                        .WithMany("Results")
                        .HasForeignKey("SeasonEventRoundRaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");

                    b.Navigation("SeasonEventRoundRace");
                });

            modelBuilder.Entity("backend.Models.SeasonTeamStanding", b =>
                {
                    b.HasOne("backend.Models.Season", null)
                        .WithMany("TeamStandings")
                        .HasForeignKey("SeasonId");

                    b.HasOne("backend.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("backend.Models.Team", b =>
                {
                    b.HasOne("backend.Models.Season", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Season");
                });

            modelBuilder.Entity("CarPot", b =>
                {
                    b.HasOne("backend.Models.Car", null)
                        .WithMany()
                        .HasForeignKey("CarsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.Pot", null)
                        .WithMany()
                        .HasForeignKey("PotsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CarSeasonEventRound", b =>
                {
                    b.HasOne("backend.Models.Car", null)
                        .WithMany()
                        .HasForeignKey("ParticipantsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Models.SeasonEventRound", null)
                        .WithMany()
                        .HasForeignKey("SeasonEventRoundsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("backend.Models.Car", b =>
                {
                    b.Navigation("HeatResults");

                    b.Navigation("OfficialName");

                    b.Navigation("Photos");

                    b.Navigation("RaceResults");

                    b.Navigation("Size");
                });

            modelBuilder.Entity("backend.Models.Circuit", b =>
                {
                    b.Navigation("Checkpoints");

                    b.Navigation("SeasonEvent")
                        .IsRequired();
                });

            modelBuilder.Entity("backend.Models.Owner", b =>
                {
                    b.Navigation("Cars");
                });

            modelBuilder.Entity("backend.Models.PicoBoard", b =>
                {
                    b.Navigation("BreakBeamSensors");
                });

            modelBuilder.Entity("backend.Models.Season", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Standings");

                    b.Navigation("TeamStandings");
                });

            modelBuilder.Entity("backend.Models.SeasonEvent", b =>
                {
                    b.Navigation("Rounds");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRound", b =>
                {
                    b.Navigation("Races");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRace", b =>
                {
                    b.Navigation("Heats");

                    b.Navigation("Results");
                });

            modelBuilder.Entity("backend.Models.SeasonEventRoundRaceHeat", b =>
                {
                    b.Navigation("Results");
                });

            modelBuilder.Entity("backend.Models.SeasonEventScoreRules", b =>
                {
                    b.Navigation("SeasonEvent")
                        .IsRequired();
                });

            modelBuilder.Entity("backend.Models.SecondChanceRules", b =>
                {
                    b.Navigation("Round")
                        .IsRequired();
                });

            modelBuilder.Entity("backend.Models.Series", b =>
                {
                    b.Navigation("OfficialNames");
                });

            modelBuilder.Entity("backend.Models.Team", b =>
                {
                    b.Navigation("Pots");
                });
#pragma warning restore 612, 618
        }
    }
}