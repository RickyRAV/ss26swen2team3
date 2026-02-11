using Microsoft.AspNetCore.Identity;

namespace TourPlanner.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
