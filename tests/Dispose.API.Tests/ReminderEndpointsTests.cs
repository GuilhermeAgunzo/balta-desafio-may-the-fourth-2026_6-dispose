using System.Net.Http.Json;
using Dispose.Core.DTOs;
using Dispose.Core.Enums;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Dispose.API.Tests;

[Collection("Dispose API")]
public sealed class ReminderEndpointsTests
{
    private readonly HttpClient _client;

    public ReminderEndpointsTests(DisposeApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task CreateReminder_and_check_endpoint_return_proximity_alert()
    {
        var createdReminder = await (await _client.PostAsJsonAsync(
            "api/reminders",
            new CreateReminderRequestDto(
                "Esquadrao Teste",
                SpecialItemCategory.Batteries,
                Guid.Parse("c0a56acd-cbd4-49eb-b937-8f287ddf2ad4"),
                1.0))).Content.ReadFromJsonAsync<ReminderDto>(JsonOptions.Default);

        var alerts = await (await _client.PostAsJsonAsync(
            "api/reminders/check",
            new ProximityCheckRequestDto(-23.5490, -46.6366))).Content.ReadFromJsonAsync<IReadOnlyList<ReminderAlertDto>>(JsonOptions.Default);

        Assert.NotNull(createdReminder);
        Assert.NotNull(alerts);
        Assert.Contains(alerts, alert => alert.ReminderId == createdReminder.Id);
    }
}
