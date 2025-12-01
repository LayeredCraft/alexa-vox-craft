using AlexaVoxCraft.Lambda.Host;
using AlexaVoxCraft.Lambda.Host.Extensions;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AwsLambda.Host.Builder;
using LayeredCraft.Logging.CompactJsonFormatter;
using Microsoft.Extensions.Hosting;
using Sample.Host.Function;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console(new CompactJsonFormatter())
        .CreateBootstrapLogger();

    Log.Information("Starting Lambda Host");
    var builder = LambdaApplication.CreateBuilder();
    // Configure Serilog as the primary logging provider
    builder.Services.AddSerilog(
        (services, lc) =>
            lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );

    builder.Services.AddSkillMediator(builder.Configuration);
    builder.Services.AddAlexaSkillHost<LambdaHandler, SkillRequest, SkillResponse>();

    await using var app = builder.Build();

    app.MapHandler(AlexaHandler.Invoke<SkillRequest, SkillResponse>);

    await app.RunAsync();
    
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}