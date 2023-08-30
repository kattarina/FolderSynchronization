using FolderSynchronisationApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System.Reflection.PortableExecutable;



var builder = WebApplication.CreateBuilder(args);// Add services to the container.c
builder.Configuration.AddCommandLine(args);


if (args == null || Array.Exists(args, arg => arg == null))
{ 
    Console.WriteLine($"One or more arguments are NULL, shutting down the application.");
    Environment.Exit(0);
}
 
if (Directory.Exists(builder.Configuration["SourceFolderPath"])) 
{
   // Console.WriteLine($"The folder '{sourceFolderPath}' exists in the system.");

    if (Directory.Exists(builder.Configuration["ReplicaFolderPath"]))
    {
       // Console.WriteLine($"The folder '{builder.Configuration["ReplicaFolderPath"]}' exists in the system."); 
    }
}

else
{
    Console.WriteLine($"The directory provided for folder comparison, does not  exists in the system. The directory will be created!"); 
}


DirectoryInfo di = Directory.CreateDirectory(builder.Configuration["LogFolderPath"]);
string fileName = "AppDebugLog.txt";
var docPath = Path.Combine(di.FullName, fileName);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .WriteTo.File(path: docPath, restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day) 
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)       
        .CreateLogger();

builder.Services.Configure<SettingConfig>(builder.Configuration);
 
builder.Services.AddScoped<IFolderSynchronizer, FolderSynchronizer>();
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

 
    