using System.Net.Http.Json;
using System.Linq;
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
        // ORS calls can fail transiently (rate limits, cold-start networking). Retry a few times.
        const int maxAttempts = 3;
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await FetchRouteAsync(from, to, profile, ct);
            }
            catch (Exception ex) when (attempt < maxAttempts && ex is not OperationCanceledException)
            {
                _logger.LogWarning("ORS route attempt {Attempt}/{Max} failed ({Message}); retrying…",
                    attempt, maxAttempts, ex.Message);
                await Task.Delay(TimeSpan.FromMilliseconds(400 * attempt), ct);
            }
        }
    }

    private async Task<RouteResult> FetchRouteAsync(string from, string to, string profile, CancellationToken ct)
    {
        // Both return raw GeoJSON [lon, lat] — no swap needed before sending to ORS
        var fromLonLat = await GeocodeAsync(from, ct);
        var toLonLat   = await GeocodeAsync(to, ct);

        _logger.LogInformation("Geocoded '{From}' → lon={FLon} lat={FLat}", from, fromLonLat[0], fromLonLat[1]);
        _logger.LogInformation("Geocoded '{To}'   → lon={TLon} lat={TLat}", to,   toLonLat[0],   toLonLat[1]);

        // ORS directions body: elevation:true adds a 3rd element [lon, lat, elevation_m] per coord
        var body = new { coordinates = new[] { fromLonLat, toLonLat }, elevation = true };

        using var req = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_opts.BaseUrl}/v2/directions/{profile}/geojson?api_key={_opts.ApiKey}")
        {
            Content = JsonContent.Create(body)
        };

        var res = await _http.SendAsync(req, ct);
        if (!res.IsSuccessStatusCode)
        {
            var errorBody = await res.Content.ReadAsStringAsync(ct);
            _logger.LogError("ORS directions {Status}: {Body}", (int)res.StatusCode, errorBody);
            res.EnsureSuccessStatusCode();
        }

        var data = await res.Content.ReadFromJsonAsync<OrsDirectionsResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Empty directions response from ORS");

        var feature = data.Features[0];
        var summary  = feature.Properties.Summary;

        // ORS returns GeoJSON [lon, lat] or [lon, lat, elevation_m]; convert to [lat, lon] for Leaflet
        var rawCoords = feature.Geometry.Coordinates;
        var latLonCoords = rawCoords.Select(c => new double[] { c[1], c[0] }).ToArray();

        // Extract elevations from 3rd element when present (elevation:true was requested)
        var elevations = rawCoords.Length > 0 && rawCoords[0].Length >= 3
            ? rawCoords.Select(c => c[2]).ToArray()
            : Array.Empty<double>();

        _logger.LogInformation(
            "ORS route fetched: {From} → {To}, {Distance:F0} m, {Duration:F0} s, {ElevPts} elevation points",
            from, to, summary.Distance, summary.Duration, elevations.Length);

        return new RouteResult(latLonCoords, elevations, summary.Distance, summary.Duration);
    }

    private async Task<double[]> GeocodeAsync(string text, CancellationToken ct)
    {
        var url = $"{_opts.BaseUrl}/geocode/search" +
                  $"?api_key={_opts.ApiKey}" +
                  $"&text={Uri.EscapeDataString(text)}" +
                  $"&size=1" +
                  $"&focus.point.lat=48.2&focus.point.lon=16.37";

        var data = await _http.GetFromJsonAsync<OrsGeocodeResponse>(url, ct)
            ?? throw new InvalidOperationException($"Empty geocode response for '{text}'");

        if (data.Features.Length == 0)
            throw new InvalidOperationException($"No geocode result for '{text}'");

        // Return raw GeoJSON [lon, lat] — no swap here
        return data.Features[0].Geometry.Coordinates;
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
