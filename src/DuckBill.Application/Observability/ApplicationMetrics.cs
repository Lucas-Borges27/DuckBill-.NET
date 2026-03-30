using System.Diagnostics.Metrics;

namespace DuckBill.Application.Observability;

public static class ApplicationMetrics
{
    public const string MeterName = "DuckBill.Application";

    private static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> RequestCount =
        Meter.CreateCounter<long>("duckbill_http_server_requests", unit: "{request}", description: "Total de requisicoes HTTP processadas.");

    public static readonly Counter<long> RequestErrorCount =
        Meter.CreateCounter<long>("duckbill_http_server_request_errors", unit: "{error}", description: "Total de requisicoes HTTP com falha.");

    public static readonly Histogram<double> RequestDurationMs =
        Meter.CreateHistogram<double>("duckbill_http_server_request_duration_ms", unit: "ms", description: "Duracao das requisicoes HTTP em milissegundos.");
}
