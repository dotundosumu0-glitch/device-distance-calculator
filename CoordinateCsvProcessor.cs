using System.Globalization;

namespace DeviceDistanceCalculator;

/// <summary>
/// Processes a simple CSV file containing pairs of geographic coordinates.
/// </summary>
public static class CoordinateCsvProcessor
{
    private const string ExpectedHeader =
        "origin_latitude,origin_longitude,destination_latitude,destination_longitude";

    /// <summary>
    /// Reads coordinate pairs from a CSV file and writes calculated distances to another CSV file.
    /// The input file must have the four documented columns in the documented order.
    /// </summary>
    public static int ProcessFile(string inputPath, string outputPath, DistanceUnit unit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        string[] lines = File.ReadAllLines(inputPath);
        if (lines.Length == 0)
            throw new FormatException("The input CSV file is empty.");

        if (!string.Equals(lines[0].Trim(), ExpectedHeader, StringComparison.OrdinalIgnoreCase))
            throw new FormatException($"The CSV header must be: {ExpectedHeader}");

        var output = new List<string>
        {
            $"{ExpectedHeader},distance,unit"
        };

        for (int index = 1; index < lines.Length; index++)
        {
            string line = lines[index].Trim();
            if (line.Length == 0)
                continue;

            string[] fields = line.Split(',');
            if (fields.Length != 4)
                throw new FormatException($"Line {index + 1}: expected exactly four comma-separated values.");

            try
            {
                double originLatitude = ParseCoordinate(fields[0]);
                double originLongitude = ParseCoordinate(fields[1]);
                double destinationLatitude = ParseCoordinate(fields[2]);
                double destinationLongitude = ParseCoordinate(fields[3]);

                var origin = new GeoCoordinate(originLatitude, originLongitude);
                var destination = new GeoCoordinate(destinationLatitude, destinationLongitude);
                double distance = GeoDistance.Calculate(origin, destination, unit);

                output.Add($"{string.Join(",", fields.Select(field => field.Trim()))},{distance.ToString("F3", CultureInfo.InvariantCulture)},{GeoDistance.GetUnitLabel(unit)}");
            }
            catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
            {
                throw new FormatException($"Line {index + 1}: {ex.Message}", ex);
            }
        }

        if (output.Count == 1)
            throw new FormatException("The input CSV contains a header but no coordinate rows.");

        File.WriteAllLines(outputPath, output);
        return output.Count - 1;
    }

    private static double ParseCoordinate(string value)
    {
        if (!double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
            || !double.IsFinite(result))
        {
            throw new FormatException($"'{value}' is not a valid finite number.");
        }

        return result;
    }
}
