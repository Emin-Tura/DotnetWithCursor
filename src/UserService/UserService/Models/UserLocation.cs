using UserService.Models;

public class UserLocation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string IpAddress { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Timezone { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime LastUpdated { get; set; }

    // Navigation property
    public User User { get; set; }
} 