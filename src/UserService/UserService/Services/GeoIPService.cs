using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace UserService.Services
{
    public class GeoIPService : IGeoIPService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeoIPService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<GeoLocationInfo> GetGeoLocationAsync(string ipAddress)
        {
            // You can use a free GeoIP API service like ip-api.com
            var response = await _httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GeoLocationInfo>();
            }

            throw new Exception("Failed to get geolocation information");
        }
    }
} 