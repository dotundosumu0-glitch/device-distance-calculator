using System.Globalization;

namespace DeviceDistanceCalculator;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0)
            return RunInteractive();

        if (args.Length == 1 && args[0] is "--help" or "-h")
            return PrintUsage(0);

        try
        {
            return RunCommandLine(args);
        }
        catch (Exception ex) when (ex is ArgumentException
                                   or FormatException
                                   or IOException
                                   or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine();
            PrintUsage(2);
            return 2;
        }
    }

    private static int RunCommandLine(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int index = 0; index < args.Length; index++)
        {
            string option = args[index];
            if (option is not ("--from" or "--to" or "--unit" or "--input" or "--output"))
                throw new ArgumentException($"Unknown option '{option}'.");

            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Option '{option}' requires a value.");

            if (!options.TryAdd(option, args[++index]))
                throw new ArgumentException($"Option '{option}' was supplied more than once.");
        }

        DistanceUnit unit = ParseUnit(options.GetValueOrDefault("--unit", "km"));
        bool hasInput = options.ContainsKey("--input");
        bool hasOutput = options.ContainsKey("--output");
        bool hasFrom = options.ContainsKey("--from");
        bool hasTo = options.ContainsKey("--to");

        if (hasInput || hasOutput)
        {
            if (!hasInput || !hasOutput || hasFrom || hasTo)
                throw new ArgumentException("Batch mode requires --input and --output, and cannot be combined with --from or --to.");

            int count = CoordinateCsvProcessor.ProcessFile(options["--input"], options["--output"], unit);
            Console.WriteLine($"Processed {count} coordinate pair(s).");
            Console.WriteLine($"Output: {Path.GetFullPath(options["--output"])}");
            return 0;
        }

        if (!hasFrom || !hasTo)
            throw new ArgumentException("Single-calculation mode requires both --from and --to.");

        GeoCoordinate origin = ParseCoordinatePair(options["--from"], "--from");
        GeoCoordinate destination = ParseCoordinatePair(options["--to"], "--to");
        double distance = GeoDistance.Calculate(origin, destination, unit);

        Console.WriteLine($"Distance: {distance.ToString("N3", CultureInfo.InvariantCulture)} {GeoDistance.GetUnitLabel(unit)}");
        Console.WriteLine("Method: Haversine (great-circle distance)");
        return 0;
    }

    private static GeoCoordinate ParseCoordinatePair(string value, string option)
    {
        string[] parts = value.Split(',');
        if (parts.Length != 2
            || !double.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude)
            || !double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude)
            || !double.IsFinite(latitude)
            || !double.IsFinite(longitude))
        {
            throw new FormatException($"'{option}' must use the format latitude,longitude (for example: 6.5244,3.3792).");
        }

        return new GeoCoordinate(latitude, longitude);
    }

    private static DistanceUnit ParseUnit(string value) => value.Trim().ToLowerInvariant() switch
    {
        "km" or "kilometres" or "kilometers" => DistanceUnit.Kilometres,
        "mi" or "miles" => DistanceUnit.Miles,
        "m" or "metres" or "meters" => DistanceUnit.Metres,
        _ => throw new ArgumentException($"Unsupported unit '{value}'. Use km, mi, or m.")
    };

    private static int RunInteractive()
    {
        Console.WriteLine("DEVICE DISTANCE CALCULATOR");
        Console.WriteLine("-------------------------");
        Console.WriteLine("Calculates great-circle distance between two geographic coordinates.\n");

        try
        {
            GeoCoordinate origin = ReadCoordinate("Origin");
            GeoCoordinate destination = ReadCoordinate("Destination");

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

    private static int PrintUsage(int exitCode)
    {
        Console.WriteLine("Device Distance Calculator");
        Console.WriteLine();
        Console.WriteLine("Interactive mode:");
        Console.WriteLine("  dotnet run");
        Console.WriteLine();
        Console.WriteLine("Calculate one distance:");
        Console.WriteLine("  dotnet run -- --from "6.5244,3.3792" --to "40.7128,-74.0060" --unit km");
        Console.WriteLine();
        Console.WriteLine("Process a CSV file:");
        Console.WriteLine("  dotnet run -- --input coordinates.csv --output distances.csv --unit mi");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --from     Origin as latitude,longitude");
        Console.WriteLine("  --to       Destination as latitude,longitude");
        Console.WriteLine("  --unit     km, mi, or m (default: km)");
        Console.WriteLine("  --input    Input CSV path for batch mode");
        Console.WriteLine("  --output   Output CSV path for batch mode");
        Console.WriteLine("  --help     Show this help");
        return exitCode;
    }
}
