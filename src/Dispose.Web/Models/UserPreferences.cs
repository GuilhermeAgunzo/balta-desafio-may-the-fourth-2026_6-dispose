namespace Dispose.Web.Models;

public sealed record UserPreferences
{
    public static UserPreferences Default { get; } = new();

    public string PreferredNeighborhood { get; set; } = "Coruscant Centro";
    public string PilotCallsign { get; set; } = "Tripulante Rebelde";
    public double ReminderRadiusKm { get; set; } = 0.75;
    public bool BrowserNotificationsEnabled { get; set; } = true;
    public bool LocationAlertsEnabled { get; set; } = true;
}
