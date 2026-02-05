using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.api.Services;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

//Env.Load("../../../.env");

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
builder.Services.AddScoped<AuditLogger>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<ISupervisorService, SupervisorService>();
builder.Services.AddScoped<IFaultService, FaultService>();



builder.Services.AddSingleton<RmqPublisher>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
