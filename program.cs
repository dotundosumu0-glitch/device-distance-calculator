using System.Globalization;

namespace DeviceDistanceCalculator;

internal static class Program
{
    private static int Main()
    {
        Console.WriteLine("DEVICE DISTANCE CALCULATOR");
        Console.WriteLine("-------------------------");
        Console.WriteLine("Calculates great-circle distance between two geographic coordinates.\n");

        try
        {
            var origin = ReadCoordinate("Origin");
            var destination = ReadCoordinate("Destination");

            Console.WriteLine("\nChoose output unit:");
            Console.WriteLine("  1. Kilometres (km)");
            Console.WriteLine("  2. Miles (mi)");
            Console.WriteLine("  3. Metres (m)");
            Console.Write("Selection [1]: ");

            string? choice = Console.ReadLine();
            DistanceUnit unit = choice?.Trim() switch
            {
                "2" => DistanceUnit.Miles,
                "3" => DistanceUnit.Metres,
                "" or "1" or null => DistanceUnit.Kilometres,
                _ => throw new ArgumentException("Choose 1, 2, or 3.")
            };

            double distance = GeoDistance.Calculate(origin, destination, unit);
            Console.WriteLine($"\nDistance: {distance:N3} {GeoDistance.GetUnitLabel(unit)}");
            Console.WriteLine("Method: Haversine (great-circle distance)");
            Console.WriteLine("\nNote: This is the surface distance over a spherical Earth, not road or travel distance.");
            return 0;
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"\nInput error: {ex.Message}");
            return 1;
        }
    }

    private static GeoCoordinate ReadCoordinate(string label)
    {
        Console.WriteLine($"{label} coordinates");

        double latitude = ReadDouble("  Latitude  (-90 to 90): ");
        double longitude = ReadDouble("  Longitude (-180 to 180): ");

        return new GeoCoordinate(latitude, longitude);
    }

    private static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                && double.IsFinite(value))
            {
                return value;
            }

            Console.WriteLine("  Enter a valid number (use a decimal point if needed).");
        }
    }
}
