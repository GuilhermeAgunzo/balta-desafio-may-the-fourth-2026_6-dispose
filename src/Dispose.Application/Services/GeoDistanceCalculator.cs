using Dispose.Application.Abstractions;

namespace Dispose.Application.Services;

public sealed class GeoDistanceCalculator : IGeoDistanceCalculator
{
    private const double EarthRadiusKilometers = 6371d;

    public double CalculateKilometers(double originLatitude, double originLongitude, double targetLatitude, double targetLongitude)
    {
        static double ToRadians(double angle) => angle * Math.PI / 180d;

        var latitudeDelta = ToRadians(targetLatitude - originLatitude);
        var longitudeDelta = ToRadians(targetLongitude - originLongitude);
        var originLatitudeRadians = ToRadians(originLatitude);
        var targetLatitudeRadians = ToRadians(targetLatitude);

        var a =
            Math.Pow(Math.Sin(latitudeDelta / 2d), 2d) +
            Math.Cos(originLatitudeRadians) *
            Math.Cos(targetLatitudeRadians) *
            Math.Pow(Math.Sin(longitudeDelta / 2d), 2d);

        var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
        return EarthRadiusKilometers * c;
    }
}
