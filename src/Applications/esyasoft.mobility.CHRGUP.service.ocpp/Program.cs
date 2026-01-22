using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;
//using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
//using OcppMicroservice.Watchdog;
////using OcppMicroservice.WebSockets;
//using esyasoft.mobility.CHRGUP.service.ocpp.Data;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.ocpp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ChargerWatchdog>();

//builder.Services.AddDbContext<OcppDbContext>(opt =>
//    opt.UseNpgsql(builder.Configuration["OcppDb:ConnectionString"]));
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration["MainDb:ConnectionString"]));

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
