using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testcontainers.PostgreSql;
using TourPlanner.DAL;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;
using TourPlanner.Models.Enums;

namespace TourPlanner.Tests.DAL;

[TestFixture]
[Category("Integration")]
public class TourRepositorySearchTests
{
    private const string UserId = "user-1";

    private PostgreSqlContainer _pg = null!;
    private TourPlannerDbContext _ctx = null!;
    private TourRepository _repo = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _pg = new PostgreSqlBuilder().WithImage("postgres:18-alpine").Build();
        try
        {
            await _pg.StartAsync();
        }
        catch (Exception ex)
        {
            Assert.Ignore($"Skipping Postgres integration tests — Docker unavailable: {ex.Message}");
        }

        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
            .UseNpgsql(_pg.GetConnectionString())
            .Options;
        _ctx = new TourPlannerDbContext(options);
        await _ctx.Database.EnsureCreatedAsync();

        _ctx.Users.Add(new ApplicationUser { Id = UserId, UserName = "tester", Email = "t@e.st" });
        await _ctx.SaveChangesAsync();

        await SeedAsync();
        _repo = new TourRepository(_ctx);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _ctx.DisposeAsync();
        await _pg.DisposeAsync();
    }

    private async Task SeedAsync()
    {
        // Lakeside Loop: VerySuitable (all Easy, short), Popularity 2, bicycle, sunset comment.
        var lakeside = Tour("Lakeside Loop", "Vienna", "Graz", TransportType.Bicycle, "A scenic ride");
        lakeside.TourLogs = new List<TourLog>
        {
            Log("Beautiful sunset over the lake", Difficulty.Easy, 5),
            Log("Easy spin", Difficulty.Easy, 6),
        };

        // Mountain Trek: Suitable (avg Medium, ≤30 km), Popularity 2, hiking.
        var mountain = Tour("Mountain Trek", "Linz", "Salzburg", TransportType.Hiking, "A tougher one");
        mountain.TourLogs = new List<TourLog>
        {
            Log("Steady climb", Difficulty.Medium, 20),
            Log("Long but doable", Difficulty.Medium, 25),
        };

        // Desert Marathon: NotSuitable (Hard, long), Popularity 2, running.
        var desert = Tour("Desert Marathon", "Rome", "Milan", TransportType.Running, "Extreme distance");
        desert.TourLogs = new List<TourLog>
        {
            Log("Brutal heat", Difficulty.Hard, 40),
            Log("Never again", Difficulty.Hard, 42),
        };

        // Empty Plan: no logs => NotSuitable, Popularity 0.
        var empty = Tour("Empty Plan", "Bregenz", "Innsbruck", TransportType.Car, "Not yet walked");

        _ctx.Tours.AddRange(lakeside, mountain, desert, empty);
        await _ctx.SaveChangesAsync();
    }

    private static Tour Tour(string name, string from, string to, TransportType transport, string description) => new()
    {
        Id = Guid.NewGuid(),
        UserId = UserId,
        Name = name,
        Description = description,
        From = from,
        To = to,
        TransportType = transport,
    };

    private static TourLog Log(string comment, Difficulty difficulty, double distanceKm) => new()
    {
        Id = Guid.NewGuid(),
        UserId = UserId,
        DateTime = DateTime.UtcNow,
        Comment = comment,
        Difficulty = difficulty,
        TotalDistanceKm = distanceKm,
        TotalTimeSeconds = 3600,
        Rating = Rating.Four,
    };

    private async Task<List<string>> SearchNames(string? query)
    {
        var result = await _repo.GetAllAsync(UserId, query);
        return result.Select(t => t.Name).ToList();
    }

    [Test]
    public async Task NoQuery_ReturnsAllToursOrderedByName()
    {
        var names = await SearchNames(null);
        Assert.That(names, Is.EqualTo(new[] { "Desert Marathon", "Empty Plan", "Lakeside Loop", "Mountain Trek" }));
    }

    [Test]
    public async Task Search_MatchesName()
    {
        Assert.That(await SearchNames("lakeside"), Is.EquivalentTo(new[] { "Lakeside Loop" }));
    }

    [Test]
    public async Task Search_IsCaseInsensitive()
    {
        Assert.That(await SearchNames("LAKESIDE"), Is.EquivalentTo(new[] { "Lakeside Loop" }));
    }

    [Test]
    public async Task Search_MatchesDestination()
    {
        Assert.That(await SearchNames("salzburg"), Is.EquivalentTo(new[] { "Mountain Trek" }));
    }

    [Test]
    public async Task Search_MatchesLogComment()
    {
        Assert.That(await SearchNames("sunset"), Is.EquivalentTo(new[] { "Lakeside Loop" }));
    }

    [Test]
    public async Task Search_MatchesTransportType()
    {
        Assert.That(await SearchNames("bicycle"), Is.EquivalentTo(new[] { "Lakeside Loop" }));
    }

    [Test]
    public async Task Search_MatchesComputedPopularity()
    {
        // Lakeside, Mountain, Desert each have 2 logs; Empty Plan has 0.
        Assert.That(await SearchNames("2"), Is.EquivalentTo(new[] { "Lakeside Loop", "Mountain Trek", "Desert Marathon" }));
        Assert.That(await SearchNames("0"), Is.EquivalentTo(new[] { "Empty Plan" }));
    }

    [Test]
    public async Task Search_MatchesComputedChildFriendliness_VerySuitable()
    {
        Assert.That(await SearchNames("very suitable"), Is.EquivalentTo(new[] { "Lakeside Loop" }));
        Assert.That(await SearchNames("verysuitable"), Is.EquivalentTo(new[] { "Lakeside Loop" }));
    }

    [Test]
    public async Task Search_MatchesComputedChildFriendliness_NotSuitable()
    {
        // Desert Marathon (Hard/long) and Empty Plan (no logs) are both NotSuitable.
        Assert.That(await SearchNames("not suitable"), Is.EquivalentTo(new[] { "Desert Marathon", "Empty Plan" }));
    }

    [Test]
    public async Task Search_NoMatch_ReturnsEmpty()
    {
        Assert.That(await SearchNames("zzz-no-match"), Is.Empty);
    }
}
