using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Services;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGeoIPService _geoIPService;
    private readonly ApplicationDbContext _context;

    public AuthController(
        IAuthService authService,
        IGeoIPService geoIPService,
        ApplicationDbContext context)
    {
        _authService = authService;
        _geoIPService = geoIPService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto registerDto)
    {
        try
        {
            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString()
                ?? Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? "Unknown";

            var geoInfo = await _geoIPService.GetGeoLocationAsync("46.101.246.13");
            var response = await _authService.LoginAsync(loginDto);

            // Get user's current location from database
            var existingLocation = await _context.UserLocations
                .FirstOrDefaultAsync(ul => ul.UserId == response.User.Id);

            bool locationChanged = existingLocation == null ||
                existingLocation.Country != geoInfo.Country ||
                existingLocation.City != geoInfo.City ||
                existingLocation.Region != geoInfo.Region ||
                existingLocation.Latitude != geoInfo.Latitude ||
                existingLocation.Longitude != geoInfo.Longitude ||
                existingLocation.IpAddress != clientIp;

            if (locationChanged)
            {
                if (existingLocation == null)
                {
                    // Create new location record
                    existingLocation = new UserLocation
                    {
                        UserId = response.User.Id
                    };
                    _context.UserLocations.Add(existingLocation);
                }

                // Update location information
                existingLocation.IpAddress = clientIp;
                existingLocation.Country = geoInfo.Country;
                existingLocation.City = geoInfo.City;
                existingLocation.Region = geoInfo.Region;
                existingLocation.Timezone = geoInfo.Timezone;
                existingLocation.Latitude = geoInfo.Latitude;
                existingLocation.Longitude = geoInfo.Longitude;
                existingLocation.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            var enrichedResponse = new
            {
                auth = response,
                location = new
                {
                    ip = clientIp,
                    country = geoInfo.Country,
                    city = geoInfo.City,
                    region = geoInfo.Region,
                    timezone = geoInfo.Timezone,
                    coordinates = new
                    {
                        latitude = geoInfo.Latitude,
                        longitude = geoInfo.Longitude
                    },
                    lastUpdated = existingLocation?.LastUpdated
                }
            };

            return Ok(enrichedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var user = await _authService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}