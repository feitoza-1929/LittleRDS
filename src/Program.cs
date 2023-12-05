using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<CommandsHandler>();
builder.Services.AddSingleton<AOFSub>();
builder.Services.AddSingleton<ServerPub>();

builder.Services.AddTransient<IRESPWriter, RESPWriter>();
builder.Services.AddTransient<IRESPReader, RESPReader>();

builder.Services.AddScoped<ServerProcessHandler>();

builder.Services.AddHostedService<Server>();
builder.Services.AddHostedService<SubStarter>();

var host = builder.Build();
host.Run();