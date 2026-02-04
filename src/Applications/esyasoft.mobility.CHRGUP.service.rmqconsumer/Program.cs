using DotNetEnv;
using esyasoft.mobility.CHRGUP.service.core.Helpers;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.rmqconsumer;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;
using Microsoft.EntityFrameworkCore;

Env.Load("../../../.env");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DBConnection")
    );
});
builder.Services.AddScoped<AuditLogger>();
builder.Services.AddScoped<AuthEvtHandler>();
builder.Services.AddScoped<StatusEvtHandler>();
builder.Services.AddScoped<MeterValueEvtHandler>();
builder.Services.AddScoped<TransactionEvtHandler>();

builder.Services.AddSingleton<RmqPublisher>();
builder.Services.AddSingleton<RmqConsumer>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
