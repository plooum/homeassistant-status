using HomeAssistant_Status;
using HomeAssistant_Status.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory) // Là où s'exécute l'app
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build());
builder.Services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddSingleton<ICertificateManager, CertificateManager>();
builder.Services.AddSingleton<IWebServerStandAlone, WebServerStandAlone>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();