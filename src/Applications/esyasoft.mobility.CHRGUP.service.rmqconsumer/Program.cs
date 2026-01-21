using esyasoft.mobility.CHRGUP.service.rmqconsumer;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Handlers;
using esyasoft.mobility.CHRGUP.service.rmqconsumer.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<AuthEvtHandler>();
builder.Services.AddScoped<StatusEvtHandler>();
builder.Services.AddScoped<MeterValueEvtHandler>();
builder.Services.AddScoped<TransactionEvtHandler>();
builder.Services.AddSingleton<RmqPublisher>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
