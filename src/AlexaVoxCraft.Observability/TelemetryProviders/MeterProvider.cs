using System.Diagnostics.Metrics;
using AlexaVoxCraft.MediatR.Observability;

namespace AlexaVoxCraft.Observability.TelemetryProviders;

public static class MeterProvider
{
    public static Meter GetAlexaVoxCraftMeter()
    {
        return AlexaVoxCraftTelemetry.Meter;
    }
}