using Dispose.AI.Abstractions;
using Dispose.AI.Configuration;
using Dispose.Application.Abstractions;
using Dispose.Core.DTOs;
using Dispose.Core.Enums;

namespace Dispose.API.Endpoints;

public static class DisposeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapDisposeApi(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api").WithTags("Dispose");

        api.MapGet("/assistant/status", (OpenAiOptions options) =>
                TypedResults.Ok(new AssistantAvailabilityDto(options.IsConfigured, "OpenAI", options.Model)))
            .WithName("GetAssistantStatus");

        api.MapGet("/neighborhoods", async (ICollectionQueryService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetNeighborhoodsAsync(cancellationToken)))
            .WithName("ListNeighborhoods");

        api.MapGet(
                "/dashboard",
                async (
                    string? neighborhood,
                    ICollectionQueryService service,
                    CancellationToken cancellationToken) =>
                {
                    var neighborhoods = await service.GetNeighborhoodsAsync(cancellationToken);
                    var selectedNeighborhood = !string.IsNullOrWhiteSpace(neighborhood)
                        ? neighborhood
                        : neighborhoods.FirstOrDefault()?.Name;

                    if (string.IsNullOrWhiteSpace(selectedNeighborhood))
                    {
                        throw new InvalidOperationException("Nenhum bairro foi configurado no catalogo inicial.");
                    }

                    var dashboard = await service.GetDashboardAsync(selectedNeighborhood, cancellationToken);
                    return TypedResults.Ok(dashboard);
                })
            .WithName("GetDashboard");

        api.MapGet(
                "/schedules",
                async (
                    string? neighborhood,
                    WasteType? wasteType,
                    ICollectionQueryService service,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(await service.GetSchedulesAsync(neighborhood, wasteType, cancellationToken)))
            .WithName("ListSchedules");

        api.MapGet(
                "/points",
                async (
                    string? neighborhood,
                    SpecialItemCategory? category,
                    double? latitude,
                    double? longitude,
                    ICollectionQueryService service,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(
                        await service.GetCollectionPointsAsync(
                            neighborhood,
                            category,
                            latitude,
                            longitude,
                            cancellationToken)))
            .WithName("ListCollectionPoints");

        api.MapGet("/reminders", async (IReminderService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetRemindersAsync(cancellationToken)))
            .WithName("ListReminders");

        api.MapPost(
                "/reminders",
                async (
                    CreateReminderRequestDto request,
                    IReminderService service,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(await service.CreateReminderAsync(request, cancellationToken)))
            .WithName("CreateReminder");

        api.MapPost(
                "/reminders/check",
                async (
                    ProximityCheckRequestDto request,
                    IReminderService service,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(await service.CheckNearbyAsync(request, cancellationToken)))
            .WithName("CheckNearbyReminders");

        api.MapPost(
                "/assistant/query",
                async (
                    AssistantRequestDto request,
                    IWasteAssistantAgent assistantAgent,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(await assistantAgent.AskAsync(request, cancellationToken)))
            .WithName("AskAssistant");

        return endpoints;
    }
}
