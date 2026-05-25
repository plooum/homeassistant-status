using System.Net;
using System.Security.Cryptography.X509Certificates;
using HomeAssistant_Status.Enums;
using HomeAssistant_Status.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HomeAssistant_Status.Services;

public class WebServerStandAlone(
    ILogger<WebServerStandAlone> logger,
    ICertificateManager certificateManager, 
    IConfiguration configuration) : IWebServerStandAlone
{
    private readonly WebApplication _app = BuildWebApp(certificateManager.CreateCertificate());
    private const int Port = 8989;

    public async Task Start()
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Mapping server requests...");

        MapRequests();
        
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Starting server...");
        
        await _app.RunAsync();
    }

    private void MapRequests()
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("Mapping /...");
        _app.MapGet("/", () => Results.Ok("ok"));
        
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("Mapping /shutdown...");
        _app.MapGet("/shutdown/{password}", (string password) =>
        {
            if (password != configuration["Password"])
                return Results.BadRequest("Mot de passe invalide !");

            try
            {
                var (filename, args) = GetCommand();
                OsCommandHelper.ExecuteCommand(filename, args, out  var stdOut, out var stdErr);
                return stdErr is not null ? throw new Exception(stdErr) : Results.Ok(stdOut);
            }
            catch (Exception e)
            {
                return Results.InternalServerError(e.Message);
            }
        });
    }

    private (string filename, string args) GetCommand()
        =>OsHelper.GetCurrentOs() switch
        {
            Os.Linux => (configuration["LinuxCommands:FileName"], configuration["LinuxCommands:Shutdown"]),
            Os.Windows => (configuration["WindowsCommands:FileName"], configuration["WindowsCommands:Shutdown"]),
            Os.MacOs => (configuration["MacOsCommands:FileName"], configuration["MacOsCommands:Shutdown"]),
            Os.Bsd => (configuration["BsdCommands:FileName"], configuration["BsdCommands:Shutdown"]),
            _ => throw new ArgumentOutOfRangeException()
        };

    private static WebApplication BuildWebApp(X509Certificate2 certificate)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, Port, listenOptions =>
            {
                listenOptions.UseHttps(certificate);
            });
        });

        return builder.Build();
    }
}