using System.Diagnostics;

namespace DuckBill.Application.Observability;

public static class Telemetry
{
    public const string ActivitySourceName = "DuckBill.Application";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
