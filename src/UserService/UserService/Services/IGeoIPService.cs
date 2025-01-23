public interface IGeoIPService
{
    Task<GeoLocationInfo> GetGeoLocationAsync(string ipAddress);
} 