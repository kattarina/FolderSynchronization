using FolderSynchronisationApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);// Add services to the container.c
builder.Configuration.AddCommandLine(args);


if (args == null || Array.Exists(args, arg => arg == null))
{ 
    Console.WriteLine($"One or more arguments are NULL, shutting down the application.");
    Environment.Exit(0);
} 

string LogFolderPath = builder.Configuration["LogFolderPath"]; 
Console.WriteLine($"LogFolderPath: {LogFolderPath}");




var dir = LogFolderPath;
 
DirectoryInfo di = Directory.CreateDirectory(dir);
string fileName = "AppDebugLog.txt";
var docPath = Path.Combine(di.FullName, fileName);


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .WriteTo.File(path: docPath, restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day) 
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)       
        .CreateLogger();

builder.Services.Configure<SettingConfig>(builder.Configuration);
 
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddHostedService<Worker>();
 


var app = builder.Build();// Configure the HTTP request pipeline.

//if (builder.Environment.IsDevelopment())
//    app.UseDeveloperExceptionPage();

//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();
//app.UseResponseCompression();
app.Run();

 