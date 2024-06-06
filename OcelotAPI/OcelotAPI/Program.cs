using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Configuration.json").AddEnvironmentVariables();


var services = builder.Services;

services.AddControllers();

// Add Ocelot
services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Use Ocelot
app.UseOcelot().Wait();

app.Run();
