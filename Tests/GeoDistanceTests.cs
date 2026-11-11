using DeviceDistanceCalculator;
using Xunit;

namespace DeviceDistanceCalculator.Tests;

public class GeoDistanceTests
{
    private static readonly GeoCoordinate Lagos = new(6.5244, 3.3792);
    private static readonly GeoCoordinate NewYork = new(40.7128, -74.0060);

    [Fact]
    public void SameCoordinate_ReturnsZero()
    {
        Assert.Equal(0, GeoDistance.Calculate(Lagos, Lagos), precision: 10);
    }

    [Fact]
    public void LagosToNewYork_ReturnsExpectedApproximateDistance()
    {
        double distance = GeoDistance.Calculate(Lagos, NewYork);
        Assert.InRange(distance, 9950, 10050);
    }

    [Fact]
    public void Distance_IsSymmetric()
    {
        double forward = GeoDistance.Calculate(Lagos, NewYork);
        double reverse = GeoDistance.Calculate(NewYork, Lagos);
        Assert.Equal(forward, reverse, precision: 8);
    }

    [Fact]
    public void SupportsMilesAndMetres()
    {
        double kilometres = GeoDistance.Calculate(Lagos, NewYork);
        Assert.Equal(kilometres * 0.621371192237334,
            GeoDistance.Calculate(Lagos, NewYork, DistanceUnit.Miles), precision: 8);
        Assert.Equal(kilometres * 1000,
            GeoDistance.Calculate(Lagos, NewYork, DistanceUnit.Metres), precision: 5);
    }

    [Theory]
    [InlineData(91, 0)]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    [InlineData(0, -181)]
    public void InvalidCoordinates_ThrowArgumentOutOfRangeException(double latitude, double longitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GeoCoordinate(latitude, longitude));
    }

    [Fact]
    public void NonFiniteCoordinates_AreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GeoCoordinate(double.NaN, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new GeoCoordinate(0, double.PositiveInfinity));
    }
}
