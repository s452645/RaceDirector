using backend.Services;
using backenend.Models;
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

app.UseCors(customOriginsConfig);

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

app.Run();
