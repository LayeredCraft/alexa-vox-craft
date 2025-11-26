using System.Diagnostics;
using AlexaVoxCraft.MediatR.Observability;

namespace AlexaVoxCraft.Observability.TelemetryProviders;

public static class ActivitySourceProvider
{
    public static ActivitySource GetAlexaVoxCraftActivitySource()
    {
        return AlexaVoxCraftTelemetry.Source;
    }
}