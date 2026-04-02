using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TourPlanner.BL.Services;

public class OrsOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openrouteservice.org";
}

public class OrsService : IOrsService
{
    private readonly HttpClient _http;
    private readonly OrsOptions _opts;
    private readonly ILogger<OrsService> _logger;

    public OrsService(HttpClient http, IOptions<OrsOptions> opts, ILogger<OrsService> logger)
    {
        _http = http;
        _opts = opts.Value;
        _logger = logger;
    }

    public async Task<RouteResult> GetRouteAsync(string from, string to, string profile, CancellationToken ct = default)
    {
        var fromCoord = await GeocodeAsync(from, ct);
        var toCoord = await GeocodeAsync(to, ct);

        // ORS directions uses [lon, lat] order (GeoJSON)
        var body = new
        {
            coordinates = new[]
            {
                new[] { fromCoord[1], fromCoord[0] },
                new[] { toCoord[1], toCoord[0] }
            }
        };

        using var req = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_opts.BaseUrl}/v2/directions/{profile}/geojson?api_key={_opts.ApiKey}")
        {
            Content = JsonContent.Create(body)
        };

        var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var data = await res.Content.ReadFromJsonAsync<OrsDirectionsResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Empty directions response from ORS");

        var feature = data.Features[0];
        var coords = feature.Geometry.Coordinates;
        var summary = feature.Properties.Summary;

        _logger.LogInformation("ORS route fetched: {From} → {To}, {Distance:F0} m, {Duration:F0} s",
            from, to, summary.Distance, summary.Duration);

        return new RouteResult(coords, summary.Distance, summary.Duration);
    }

    private async Task<double[]> GeocodeAsync(string text, CancellationToken ct)
    {
        var url = $"{_opts.BaseUrl}/geocode/search?api_key={_opts.ApiKey}&text={Uri.EscapeDataString(text)}&size=1";
        var data = await _http.GetFromJsonAsync<OrsGeocodeResponse>(url, ct)
            ?? throw new InvalidOperationException($"Empty geocode response for '{text}'");

        if (data.Features.Length == 0)
            throw new InvalidOperationException($"No geocode result for '{text}'");

        // GeoJSON coordinates are [lon, lat] — return as [lat, lon] for consistency
        var c = data.Features[0].Geometry.Coordinates;
        return [c[1], c[0]];
    }

    private sealed class OrsGeocodeResponse
    {
        public OrsGeocodeFeature[] Features { get; set; } = [];
    }

    private sealed class OrsGeocodeFeature
    {
        public OrsGeocodeGeometry Geometry { get; set; } = new();
    }

    private sealed class OrsGeocodeGeometry
    {
        public double[] Coordinates { get; set; } = [];
    }

    private sealed class OrsDirectionsResponse
    {
        public OrsDirectionsFeature[] Features { get; set; } = [];
    }

    private sealed class OrsDirectionsFeature
    {
        public OrsDirectionsGeometry Geometry { get; set; } = new();
        public OrsDirectionsProperties Properties { get; set; } = new();
    }

    private sealed class OrsDirectionsGeometry
    {
        // Each element is [lon, lat]
        public double[][] Coordinates { get; set; } = [];
    }

    private sealed class OrsDirectionsProperties
    {
        public OrsSummary Summary { get; set; } = new();
    }

    private sealed class OrsSummary
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
    }
}
