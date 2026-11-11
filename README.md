# Device Distance Calculator

A small C#/.NET 8 console application that calculates the great-circle distance between two geographic coordinates using the **Haversine formula**.

This project started as a hardcoded Lagos-to-New York calculation. It is being developed into a reusable, validated and tested coordinate-distance tool.

## Features

- Interactive latitude/longitude input for two locations
- Coordinate validation (latitude: -90° to 90°; longitude: -180° to 180°)
- Results in kilometres, miles or metres
- Haversine calculation using a mean spherical Earth radius of 6,371.0088 km
- Floating-point safeguards for the Haversine intermediate value
- Automated unit tests for core calculations and invalid input

## Requirements

- .NET 8 SDK

Check your installation:

```bash
dotnet --version
```

## Run

Clone the repository, enter its directory, and run:

```bash
git clone https://github.com/dotundosumu0-glitch/device-distance-calculator.git
cd device-distance-calculator
dotnet run
```

Enter the latitude and longitude of the origin and destination in decimal degrees. Use a decimal point for fractional values. Then select the desired output unit.

### Example coordinates

| Location | Latitude | Longitude |
| --- | ---: | ---: |
| Lagos, Nigeria | 6.5244 | 3.3792 |
| New York, USA | 40.7128 | -74.0060 |

The result is approximately 10,000 km (the exact displayed value is calculated by the application).

## Run tests

```bash
dotnet test Tests/DeviceDistanceCalculator.Tests.csproj
```

The test suite covers identical points, a known city-pair range, symmetry, unit conversion, and invalid/non-finite coordinates.

## Project structure

```text
.
├── DeviceDistanceCalculator.csproj
├── GeoDistance.cs
├── program.cs
├── README.md
└── Tests/
    ├── DeviceDistanceCalculator.Tests.csproj
    └── GeoDistanceTests.cs
```

## How it works

The Haversine formula calculates the central angle between two points on a sphere from their latitude and longitude:

```text
a = sin²(Δφ / 2) + cos(φ1) cos(φ2) sin²(Δλ / 2)
c = 2 atan2(√a, √(1 − a))
distance = R × c
```

Here, φ represents latitude in radians, λ represents longitude in radians, and R is the mean spherical Earth radius. The implementation clamps the intermediate value to [0, 1] to protect against floating-point rounding near edge cases.

## Limitations

- Assumes a spherical Earth; it is an approximation rather than an ellipsoidal geodesic calculation.
- Calculates straight-line surface distance, not road distance, travel time or live device-to-device range.
- Coordinates must be supplied in decimal degrees.

## Roadmap

- [ ] Add command-line arguments for scripting and automation
- [ ] Add CSV/batch coordinate processing
- [ ] Compare spherical results with an ellipsoidal geodesic implementation
- [ ] Publish benchmark and accuracy notes

## License

No license has been specified yet. Until one is added, normal copyright restrictions apply.
