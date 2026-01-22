using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.api.Services;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DBConnection");
    

    if (string.IsNullOrEmpty(conn))
        throw new InvalidOperationException("Connection string not found.");

    options.UseNpgsql(
        conn,
        b => b.MigrationsAssembly("esyasoft.mobility.CHRGUP.service.persistence")
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IChargerService, ChargerService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IChargingSessionService, ChargingSessionService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IMessagingPublisher, NoOpMessagingPublisher>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
   
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
