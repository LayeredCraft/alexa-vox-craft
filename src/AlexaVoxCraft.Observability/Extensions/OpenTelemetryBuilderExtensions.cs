using AlexaVoxCraft.MediatR.Observability;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace AlexaVoxCraft.Observability.Extensions;

public static class OpenTelemetryBuilderExtensions
{
    public static TracerProviderBuilder AddAlexaVoxCraftInstrumentation(this TracerProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        return builder.AddSource(AlexaVoxCraftTelemetry.ActivitySourceName);
    }

    public static MeterProviderBuilder AddAlexaVoxCraftInstrumentation(this MeterProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        return builder.AddMeter(AlexaVoxCraftTelemetry.MeterName);
    }
}