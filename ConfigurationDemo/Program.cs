using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VaultSharp.Extensions.Configuration;
using VaultSharp.V1.AuthMethods.UserPass;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//create vault options. Configuration validation has been overlooked in this example. 
var vaultOptions = new VaultOptions(
    configuration["Vault:Address"], 
    new UserPassAuthMethodInfo(
        configuration["Vault:Username"],
        configuration["Vault:Password"]), //feel free to try out other auth methods
    reloadOnChange:true,
    reloadCheckIntervalSeconds:int.Parse(configuration["Vault:ReloadSeconds"])
    );

configuration.AddVaultConfiguration(
    () => vaultOptions,
    configuration["Vault:BasePath"],
    configuration["Vault:MountPoint"]);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//change watcher to auto reload configuration on change
builder.Services.AddHostedService<VaultChangeWatcher>();

//configure options
builder.Services.Configure<ApplicationSetting>(configuration.GetSection(ApplicationSetting.Name));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/view-config", ([FromServices] IOptionsMonitor<ApplicationSetting> optionsMonitor) =>
    {
        var setting = optionsMonitor.CurrentValue;
        return Results.Ok(setting);
    })
    .WithName("ViewConfig")
    .WithOpenApi();

app.Run();

public class ApplicationSetting
{
    public const string Name = "ApplicationSetting";
    public string? DisplayTitle { get; set; }
    public bool IsLive { get; set; }
}