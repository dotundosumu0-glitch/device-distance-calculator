# Device Distance Calculator

A C#/.NET 8 console application that calculates the great-circle distance between geographic coordinates using the **Haversine formula**.

This project started as a hardcoded Lagos-to-New York calculation. It is being developed into a reusable, validated and tested coordinate-distance tool.

## Features

- Interactive coordinate entry
- Command-line calculation for scripting and automation
- CSV batch processing for multiple coordinate pairs
- Coordinate validation (latitude: -90° to 90°; longitude: -180° to 180°)
- Results in kilometres, miles or metres
- Haversine calculation using a mean spherical Earth radius of 6,371.0088 km
- Floating-point safeguards and automated tests

## Requirements

- .NET 8 SDK

Check your installation:

```bash
dotnet --version
```

## Run interactively

```bash
dotnet run
```

Enter the latitude and longitude of the origin and destination in decimal degrees, then select the output unit.

## Command-line mode

Calculate a single distance without interactive prompts:

```bash
dotnet run -- --from "6.5244,3.3792" --to "40.7128,-74.0060" --unit km
```

The coordinate format is `latitude,longitude`. The unit is optional and defaults to kilometres.

Supported units:

| Value | Unit |
| --- | --- |
| `km` | Kilometres |
| `mi` | Miles |
| `m` | Metres |

Show all command options:

```bash
dotnet run -- --help
```

## CSV batch mode

Create an input file named `coordinates.csv` with this exact header and column order:

```csv
origin_latitude,origin_longitude,destination_latitude,destination_longitude
6.5244,3.3792,40.7128,-74.0060
6.5244,3.3792,9.0765,7.3986
```

Run the batch processor:

```bash
dotnet run -- --input coordinates.csv --output distances.csv --unit km
```

The output file contains the original four coordinate columns followed by `distance` and `unit`. Each non-empty data row is validated before the output file is written. If a row is invalid, the error identifies its line number and no partial results file is produced.

The CSV format is intentionally numeric and simple: four comma-separated coordinate values per row, with no extra columns or quoted fields.

## Example coordinates

| Location | Latitude | Longitude |
| --- | ---: | ---: |
| Lagos, Nigeria | 6.5244 | 3.3792 |
| New York, USA | 40.7128 | -74.0060 |
| Abuja, Nigeria | 9.0765 | 7.3986 |

## Run tests

```bash
dotnet test Tests/DeviceDistanceCalculator.Tests.csproj
```

Tests cover identical points, a known city-pair range, symmetry, unit conversion, invalid/non-finite coordinates, CSV output, and invalid batch rows.

## Project structure

```text
.
├── DeviceDistanceCalculator.csproj
├── GeoDistance.cs
├── CoordinateCsvProcessor.cs
├── program.cs
├── README.md
└── Tests/
    ├── DeviceDistanceCalculator.Tests.csproj
    ├── GeoDistanceTests.cs
    └── CoordinateCsvProcessorTests.cs
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

- Assumes a spherical Earth; this is an approximation rather than an ellipsoidal geodesic calculation.
- Calculates straight-line surface distance, not road distance, travel time or live device-to-device range.
- Coordinates must be supplied in decimal degrees.

## Roadmap

- [ ] Compare spherical results with an ellipsoidal geodesic implementation
- [ ] Publish benchmark and accuracy notes
- [ ] Add optional named locations to batch records

## License

No license has been specified yet. Until one is added, normal copyright restrictions apply.
