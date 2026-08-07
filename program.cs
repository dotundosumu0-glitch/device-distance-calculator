using System;

class Program
{
    static void Main()
    {
        // Example coordinates (latitude, longitude)
        double lat1 = 6.5244;   // Lagos
        double lon1 = 3.3792;
        double lat2 = 40.7128;  // New York
        double lon2 = -74.0060;

        double distance = Haversine(lat1, lon1, lat2, lon2);
        Console.WriteLine($"Distance: {distance} km");
    }

    static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        double dLat = (lat2 - lat1) * Math.PI / 180.0;
        double dLon = (lon2 - lon1) * Math.PI / 180.0;

        lat1 = lat1 * Math.PI / 180.0;
        lat2 = lat2 * Math.PI / 180.0;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
