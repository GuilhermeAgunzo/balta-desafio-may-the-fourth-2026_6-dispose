namespace Dispose.Application.Abstractions;

public interface IGeoDistanceCalculator
{
    double CalculateKilometers(double originLatitude, double originLongitude, double targetLatitude, double targetLongitude);
}
