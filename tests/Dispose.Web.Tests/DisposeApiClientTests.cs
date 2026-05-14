using System.Net;
using System.Text;
using Dispose.Core.Enums;
using Dispose.Web.Services;

namespace Dispose.Web.Tests;

public sealed class DisposeApiClientTests
{
    [Fact]
    public async Task GetDashboardAsync_deserializes_string_enums_from_api()
    {
        const string dashboardPayload =
            """
            {
              "neighborhoodName": "Coruscant Centro",
              "sector": "Setor Senado",
              "motto": "Teste",
              "upcomingCollections": [
                {
                  "scheduleId": "11111111-1111-1111-1111-111111111111",
                  "wasteType": "Glass",
                  "wasteTypeLabel": "Vidro",
                  "dayOfWeek": "Friday",
                  "dayLabel": "Sexta",
                  "nextCollectionDate": "2026-05-15",
                  "pickupWindow": "08:00-10:00",
                  "routeCode": "ROT-1",
                  "guidance": "Embale o vidro"
                }
              ],
              "featuredCollectionPoints": [
                {
                  "id": "22222222-2222-2222-2222-222222222222",
                  "name": "Hangar Verde",
                  "neighborhoodName": "Coruscant Centro",
                  "address": "Rua 1",
                  "landmark": "Base",
                  "factionTag": "Alianca",
                  "latitude": -23.55,
                  "longitude": -46.63,
                  "acceptedCategories": [ "Batteries" ],
                  "acceptedCategoryLabels": [ "Pilhas e baterias" ],
                  "distanceKm": 0.4
                }
              ],
              "activeReminderCount": 2,
              "generatedAt": "2026-05-14T02:00:00+00:00"
            }
            """;

        using var httpClient = new HttpClient(new StubHttpMessageHandler(dashboardPayload))
        {
            BaseAddress = new Uri("https://localhost/")
        };

        var client = new DisposeApiClient(httpClient);

        var dashboard = await client.GetDashboardAsync("Coruscant Centro");

        Assert.Equal(WasteType.Glass, dashboard.UpcomingCollections[0].WasteType);
        Assert.Equal(SpecialItemCategory.Batteries, dashboard.FeaturedCollectionPoints[0].AcceptedCategories[0]);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _content;

        public StubHttpMessageHandler(string content)
        {
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}
