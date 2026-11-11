using DeviceDistanceCalculator;
using Xunit;

namespace DeviceDistanceCalculator.Tests;

public class CoordinateCsvProcessorTests
{
    [Fact]
    public void ProcessFile_WritesCalculatedDistanceAndUnit()
    {
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string input = Path.Combine(directory, "coordinates.csv");
        string output = Path.Combine(directory, "distances.csv");

        try
        {
            File.WriteAllLines(input, new[]
            {
                "origin_latitude,origin_longitude,destination_latitude,destination_longitude",
                "6.5244,3.3792,40.7128,-74.0060"
            });

            int processed = CoordinateCsvProcessor.ProcessFile(input, output, DistanceUnit.Miles);

            string[] result = File.ReadAllLines(output);
            Assert.Equal(1, processed);
            Assert.Equal("origin_latitude,origin_longitude,destination_latitude,destination_longitude,distance,unit", result[0]);
            Assert.EndsWith(",mi", result[1]);
            Assert.Contains("mi", result[1]);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void ProcessFile_InvalidRowDoesNotCreateOutput()
    {
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string input = Path.Combine(directory, "coordinates.csv");
        string output = Path.Combine(directory, "distances.csv");

        try
        {
            File.WriteAllLines(input, new[]
            {
                "origin_latitude,origin_longitude,destination_latitude,destination_longitude",
                "6.5244,3.3792,not-a-number,-74.0060"
            });

            Assert.Throws<FormatException>(() =>
                CoordinateCsvProcessor.ProcessFile(input, output, DistanceUnit.Kilometres));
            Assert.False(File.Exists(output));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
