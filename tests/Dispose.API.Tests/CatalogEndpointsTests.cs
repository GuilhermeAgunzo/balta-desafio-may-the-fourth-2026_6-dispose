using System.Net.Http.Json;
using Dispose.Core.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Dispose.API.Tests;

[Collection("Dispose API")]
public sealed class CatalogEndpointsTests
{
    private readonly HttpClient _client;

    public CatalogEndpointsTests(DisposeApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task GetSchedules_returns_seeded_rows_for_selected_neighborhood()
    {
        var schedules = await _client.GetFromJsonAsync<IReadOnlyList<CollectionScheduleDto>>(
            "api/schedules?neighborhood=Coruscant%20Centro",
            JsonOptions.Default);

        Assert.NotNull(schedules);
        Assert.True(schedules.Count >= 4);
        Assert.Contains(schedules, item => item.NeighborhoodName == "Coruscant Centro");
    }

    [Fact]
    public async Task GetAssistantStatus_returns_openai_configuration_state()
    {
        var status = await _client.GetFromJsonAsync<AssistantAvailabilityDto>("api/assistant/status", JsonOptions.Default);

        Assert.NotNull(status);
        Assert.False(status.IsConfigured);
        Assert.Equal("OpenAI", status.Provider);
    }
}
