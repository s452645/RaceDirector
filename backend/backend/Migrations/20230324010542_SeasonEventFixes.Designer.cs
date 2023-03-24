﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using backenend.Models;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(BackendContext))]
    [Migration("20230324010542_SeasonEventFixes")]
    partial class SeasonEventFixes
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

                    b.Property<Guid>("BreakBeamSensorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CircuitId")
                        .HasColumnType("uniqueidentifier");

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

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CircuitId");

                    b.HasIndex("SeasonId");

                    b.ToTable("SeasonEvents");
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
                        .HasForeignKey("BreakBeamSensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
                        .WithMany()
                        .HasForeignKey("CircuitId");

                    b.HasOne("backend.Models.Season", "Season")
                        .WithMany("Events")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Circuit");

                    b.Navigation("Season");
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

            modelBuilder.Entity("backend.Models.Car", b =>
                {
                    b.Navigation("OfficialName");

                    b.Navigation("Photos");

                    b.Navigation("Size");
                });

            modelBuilder.Entity("backend.Models.Circuit", b =>
                {
                    b.Navigation("Checkpoints");
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
