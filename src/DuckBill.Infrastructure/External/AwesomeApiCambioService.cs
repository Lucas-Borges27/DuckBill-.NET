using System.Net.Http.Json;
using DuckBill.Domain.Interfaces;

namespace DuckBill.Infrastructure.External;

public class AwesomeApiCambioService : ICambioService
{
    private readonly HttpClient _http;
    public AwesomeApiCambioService(HttpClient http)
    {
        _http = http;
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("DuckBill/1.0");
    }

    public async Task<decimal> ConverterAsync(decimal valor, string de, string para, CancellationToken ct = default)
    {
        de = de.ToUpperInvariant();
        para = para.ToUpperInvariant();
        if (de == para) return valor;

        var pair = $"{de}-{para}";
        var resp = await _http.GetFromJsonAsync<Dictionary<string, AwesomeQuote>>($"last/{pair}", ct)
                   ?? throw new InvalidOperationException("Resposta de câmbio vazia.");

        var key = (de + para).ToUpperInvariant(); // ex: USDBRL
        if (!resp.TryGetValue(key, out var quote) ||
            !decimal.TryParse(quote.Bid, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var bid))
            throw new InvalidOperationException("Não foi possível obter taxa de câmbio.");

        return valor * bid;
    }

    public class AwesomeQuote
    {
        public string Code { get; set; } = default!;
        public string Codein { get; set; } = default!;
        public string Bid { get; set; } = default!; // "5.1234"
    }
}
