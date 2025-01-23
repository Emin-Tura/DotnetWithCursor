using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MaxMind.GeoIP2;

namespace GeoIPService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeoIPController : ControllerBase
    {
        private readonly string _countryDbPath;
        private readonly string _asnDbPath;

        public GeoIPController()
        {
            // Veritabanı dosyalarının yollarını belirle
            _countryDbPath = Path.Combine(Directory.GetCurrentDirectory(), "GeoData", "GeoLite2-Country.mmdb");
            _asnDbPath = Path.Combine(Directory.GetCurrentDirectory(), "GeoData", "GeoLite2-ASN.mmdb");
        }

        [HttpGet("GetGeoInfoByIP")]
        public IActionResult GetGeoInfoByIP([FromQuery] string ip = null)
        {
            // Test amaçlı varsayılan IP adresi
            var ipAddress = ip ?? "88.224.122.145";

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return BadRequest("Invalid or missing IP address.");
            }

            try
            {
                // Veritabanından bilgileri al
                var countryInfo = GetCountryInfo(ipAddress);
                var asnInfo = GetAsnInfo(ipAddress);
                // Bilgileri birleştir ve döndür
                return Ok(new
                {
                    IP = ipAddress,
                    Country = countryInfo,
                    ASN = asnInfo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        private object GetCountryInfo(string ip)
        {
            using var reader = new DatabaseReader(_countryDbPath);
            var countryResponse = reader.Country(ip);

            return new
            {
                Name = countryResponse.Country.Name,
                IsoCode = countryResponse.Country.IsoCode
            };
        }

        private object GetAsnInfo(string ip)
        {

            using var reader = new DatabaseReader(_asnDbPath);
            var asnResponse = reader.Asn(ip);

            return new
            {
                AutonomousSystemNumber = asnResponse.AutonomousSystemNumber,
                AutonomousSystemOrganization = asnResponse.AutonomousSystemOrganization
            };
        }
    }
}
