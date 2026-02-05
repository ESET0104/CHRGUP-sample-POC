using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.Services;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;

//Env.Load("../../../.env");


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ChargerWatchdog>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DBConnection")
    );
});

builder.Services.AddScoped<ChargerAuthService>();

var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", wsApp =>
{
    wsApp.UseMiddleware<WebSocketMiddleware>();
});

_ = RabbitMqConnection.Channel;

var commandConsumer = new RabbitMqConsumer(RabbitMqConnection.Channel);
commandConsumer.Start();

app.Run();
