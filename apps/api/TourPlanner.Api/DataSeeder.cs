using Microsoft.AspNetCore.Identity;
using TourPlanner.DAL;
using TourPlanner.Models;
using TourPlanner.Models.Enums;

namespace TourPlanner.Api;

/// <summary>
/// Seeds the database with demo data for local development.
/// Only runs when the DB is empty — safe to call on every startup.
/// Demo credentials: demo@tourplanner.dev / Demo123!
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<TourPlannerDbContext>();

        if (db.Tours.Any()) return;

        const string email = "demo@tourplanner.dev";
        const string password = "Demo123!";

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = "demo",
                Email = email,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"Seeder: failed to create demo user — {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var tours = new List<Tour>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Wienerwald Ridge Hike",
                Description = "A classic day hike through the Vienna Woods with panoramic views over the city.",
                From = "Vienna, Hütteldorf",
                To = "Klosterneuburg",
                TransportType = TransportType.Hiking,
                Distance = 18.4,
                EstimatedTimeSeconds = 18000,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Danube Cycle Path",
                Description = "Flat riverside cycling along the Donauradweg — great for a relaxed weekend ride.",
                From = "Vienna, Reichsbrücke",
                To = "Krems an der Donau",
                TransportType = TransportType.Bicycle,
                Distance = 82.1,
                EstimatedTimeSeconds = 14400,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Prater Morning Run",
                Description = "An easy flat run along the Hauptallee in the Prater. Perfect for a morning session.",
                From = "Vienna, Praterstern",
                To = "Vienna, Lusthaus",
                TransportType = TransportType.Running,
                Distance = 4.7,
                EstimatedTimeSeconds = 1500,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Salzkammergut Lake District",
                Description = "A multi-day road trip through Austria's lake district — Hallstatt, Wolfgangsee, Traunsee.",
                From = "Vienna",
                To = "Salzburg",
                TransportType = TransportType.Car,
                Distance = 310.0,
                EstimatedTimeSeconds = 28800,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        db.Tours.AddRange(tours);

        var logs = new List<TourLog>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TourId = tours[0].Id,
                UserId = user.Id,
                DateTime = DateTime.UtcNow.AddDays(-14),
                Comment = "Great weather, trail was a bit muddy after the rain but still enjoyable.",
                Difficulty = Difficulty.Medium,
                TotalDistanceKm = 18.4,
                TotalTimeSeconds = 19200,
                Rating = Rating.Four,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                TourId = tours[0].Id,
                UserId = user.Id,
                DateTime = DateTime.UtcNow.AddDays(-60),
                Comment = "First time doing this route. Stunning views at the top, will definitely do again.",
                Difficulty = Difficulty.Medium,
                TotalDistanceKm = 18.4,
                TotalTimeSeconds = 21600,
                Rating = Rating.Five,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                TourId = tours[1].Id,
                UserId = user.Id,
                DateTime = DateTime.UtcNow.AddDays(-7),
                Comment = "Tailwind almost the whole way. Took the ferry back from Krems, highly recommended.",
                Difficulty = Difficulty.Easy,
                TotalDistanceKm = 82.1,
                TotalTimeSeconds = 13500,
                Rating = Rating.Five,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                TourId = tours[1].Id,
                UserId = user.Id,
                DateTime = DateTime.UtcNow.AddDays(-45),
                Comment = "Strong headwind made the second half tough. Beautiful scenery though.",
                Difficulty = Difficulty.Medium,
                TotalDistanceKm = 82.1,
                TotalTimeSeconds = 16200,
                Rating = Rating.Three,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                TourId = tours[2].Id,
                UserId = user.Id,
                DateTime = DateTime.UtcNow.AddDays(-3),
                Comment = "Easy recovery run. Legs felt good.",
                Difficulty = Difficulty.Easy,
                TotalDistanceKm = 4.7,
                TotalTimeSeconds = 1380,
                Rating = Rating.Four,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                TourId = tours[2].Id,
                UserId = user.Id,
                DateTime = DateTime.UtcNow.AddDays(-10),
                Comment = "Did two laps today. Very busy on a Sunday morning but the flat course is nice.",
                Difficulty = Difficulty.Easy,
                TotalDistanceKm = 9.4,
                TotalTimeSeconds = 2820,
                Rating = Rating.Four,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                TourId = tours[3].Id,
                UserId = user.Id,
                DateTime = DateTime.UtcNow.AddDays(-30),
                Comment = "Hallstatt was packed with tourists but still worth it. Road conditions excellent.",
                Difficulty = Difficulty.Easy,
                TotalDistanceKm = 310.0,
                TotalTimeSeconds = 30600,
                Rating = Rating.Four,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        db.TourLogs.AddRange(logs);
        await db.SaveChangesAsync();
    }
}
