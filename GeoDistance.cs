namespace DeviceDistanceCalculator;

/// <summary>
/// A geographic position expressed in decimal degrees.
/// </summary>
public readonly record struct GeoCoordinate
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoCoordinate(double latitude, double longitude)
    {
        if (!double.IsFinite(latitude) || latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");

        if (!double.IsFinite(longitude) || longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");

        Latitude = latitude;
        Longitude = longitude;
    }
}

public enum DistanceUnit
{
    Kilometres,
    Miles,
    Metres
}

/// <summary>
/// Calculates great-circle distances using the Haversine formula.
/// </summary>
public static class GeoDistance
{
    // Mean spherical Earth radius, in kilometres (IUGG convention).
    private const double EarthRadiusKm = 6371.0088;

    public static double Calculate(
        GeoCoordinate origin,
        GeoCoordinate destination,
        DistanceUnit unit = DistanceUnit.Kilometres)
    {
        double lat1 = ToRadians(origin.Latitude);
        double lat2 = ToRadians(destination.Latitude);
        double deltaLat = lat2 - lat1;
        double deltaLon = ToRadians(destination.Longitude - origin.Longitude);

        double sinLat = Math.Sin(deltaLat / 2);
        double sinLon = Math.Sin(deltaLon / 2);

        double a = sinLat * sinLat
                 + Math.Cos(lat1) * Math.Cos(lat2) * sinLon * sinLon;

        // Floating-point rounding can move a a tiny amount outside [0, 1].
        a = Math.Clamp(a, 0.0, 1.0);

        double centralAngle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double kilometres = EarthRadiusKm * centralAngle;

        return unit switch
        {
            DistanceUnit.Kilometres => kilometres,
            DistanceUnit.Miles => kilometres * 0.621371192237334,
            DistanceUnit.Metres => kilometres * 1000,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported distance unit.")
        };
    }

    public static string GetUnitLabel(DistanceUnit unit) => unit switch
    {
        DistanceUnit.Kilometres => "km",
        DistanceUnit.Miles => "mi",
        DistanceUnit.Metres => "m",
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported distance unit.")
    };

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);
}
