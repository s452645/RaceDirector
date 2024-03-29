using backend.Models;
using backend.Services.Cars;
using backend.Services.Hardware;
using backend.Services.Hardware.Comms;
using backend.Services.Seasons;
using backend.Services.Seasons.Events.Circuits;
using backend.Services.Seasons.Events.Rounds;
using backend.Services.Seasons.Events.Rounds.Races;
using Microsoft.EntityFrameworkCore;

var customOriginsConfig = "_customOriginsConfig";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: customOriginsConfig,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddSingleton<HardwareCommunicationService>();
builder.Services.AddSingleton<TimeSyncService>();
builder.Services.AddSingleton<BoardsManager>();
builder.Services.AddSingleton<BoardEventsService>();
builder.Services.AddSingleton<SeasonEventRoundRaceService>();

builder.Services.AddScoped<SeasonService>();
builder.Services.AddScoped<CircuitService>();
builder.Services.AddScoped<PicoBoardsService>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<SeasonEventRoundService>();

builder.Services.AddControllers();

builder.Services.AddDbContext<BackendContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("BackendContext"))
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseCors(customOriginsConfig);

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

app.Run();
