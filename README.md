# CountryCodeKit

[![NuGet](https://img.shields.io/nuget/v/CountryCodeKit.svg)](https://www.nuget.org/packages/CountryCodeKit/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CountryCodeKit.svg)](https://www.nuget.org/packages/CountryCodeKit/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A comprehensive .NET library for converting between different country code formats, names, and other country-related information. CountryCodeKit offers robust support for ISO 3166-1 standards along with phone codes, localized names, and geographical classifications.

## Features

- **Comprehensive Coverage**
  - ISO 3166-1 alpha-2 codes (two-letter)
  - ISO 3166-1 alpha-3 codes (three-letter)
  - ISO 3166-1 numeric codes
  - Country names (English and localized)
  - Official country names
  - International phone calling codes
  - Regional and subregional classifications

- **Performance Optimization**
  - Specify input format for faster processing
  - Optimized batch conversion for multiple inputs
  - Lazy loading of country data

- **Flexible Matching**
  - Multiple identifier search
  - Exact and partial matching
  - Phone number parsing
  - Alternative spellings and names

- **Data Rich**
  - Complete data for all countries and territories
  - Localized names in 8+ languages
  - Geographic hierarchies
  - Alternative names and abbreviations

- **Customizable**
  - JSON-based data loading
  - Custom country database support
  - Runtime database updates

## Installation

### Via NuGet Package Manager
```
Install-Package CountryCodeKit
```

### Via .NET CLI
```
dotnet add package CountryCodeKit
```

### Via Package Reference in .csproj
```xml
<PackageReference Include="CountryCodeKit" Version="1.0.0" />
```

## Quick Start

```csharp
using CountryCodeKit;

// Convert from ISO 3166-1 alpha-2 to country name
string countryName = CountryMapper.Convert("US", CountryCodeFormat.Name);
// Result: "United States"

// Convert from country name to alpha-3
string alpha3Code = CountryMapper.Convert("Germany", CountryCodeFormat.Alpha3);
// Result: "DEU"

// Check if a string is a valid country identifier
bool isValid = CountryMapper.IsValidCountry("UK");
// Result: true
```

## Code Examples

### Basic Conversion

```csharp
// Convert between different formats
string countryName = CountryMapper.Convert("FR", CountryCodeFormat.Name);
string alpha2Code = CountryMapper.Convert("Germany", CountryCodeFormat.Alpha2);
string numericCode = CountryMapper.Convert("JPN", CountryCodeFormat.Numeric);
string officialName = CountryMapper.Convert("840", CountryCodeFormat.OfficialName);

// Performance-optimized conversion (when input format is known)
string optimizedResult = CountryMapper.Convert(
    "US",                      // Input value
    CountryCodeFormat.Alpha2,  // Input format
    CountryCodeFormat.Name     // Output format
);
```

### Phone Calling Codes

```csharp
// Get phone calling code for a country
string callingCode = CountryMapper.Convert("United States", CountryCodeFormat.CallingCode);
// Result: "1"

// Get calling code with + prefix
string internationalFormat = CountryMapper.Convert("Japan", CountryCodeFormat.CallingCodeWithPlus);
// Result: "+81"

// Identify country from phone number prefix
string country = CountryMapper.Convert("+44", CountryCodeFormat.Name);
// Result: "United Kingdom"
```

### Localized Names

```csharp
// Get country name in different languages
string spanishName = CountryMapper.Convert("DE", CountryCodeFormat.LocalizedName, "es");
// Result: "Alemania"

string frenchName = CountryMapper.Convert("JP", CountryCodeFormat.LocalizedName, "fr");
// Result: "Japon"

string arabicName = CountryMapper.Convert("IT", CountryCodeFormat.LocalizedName, "ar");
// Result: "إيطاليا"
```

### Batch Processing

```csharp
// Convert multiple inputs at once
var inputs = new List<string> { "US", "GB", "FR", "DE", "JP" };
var results = CountryMapper.BatchConvert(inputs, CountryCodeFormat.Name);

// With known input format for better performance
var alpha2Codes = new List<string> { "US", "GB", "CA", "AU", "NZ" };
var optimizedResults = CountryMapper.BatchConvert(
    alpha2Codes, 
    CountryCodeFormat.Alpha2,  // Input format
    CountryCodeFormat.Alpha3   // Output format
);
```

### Working with Country Objects

```csharp
// Get complete country information
Country france = CountryMapper.FindCountry("France");

Console.WriteLine($"Name: {france.Name}");
Console.WriteLine($"Official Name: {france.OfficialName}");
Console.WriteLine($"Alpha-2: {france.Alpha2Code}");
Console.WriteLine($"Alpha-3: {france.Alpha3Code}");
Console.WriteLine($"Numeric Code: {france.NumericCode}");
Console.WriteLine($"Phone Code: +{france.CallingCode}");
Console.WriteLine($"Region: {france.Region}");
Console.WriteLine($"Subregion: {france.Subregion}");

// Access localized names
Console.WriteLine($"In German: {france.LocalizedNames["de"]}");
Console.WriteLine($"In Japanese: {france.LocalizedNames["ja"]}");

// Check alternative names
foreach (var altName in france.AlternativeNames)
{
    Console.WriteLine($"Also known as: {altName}");
}
```

### Geographic Information

```csharp
// Get all countries in a region
List<Country> europeanCountries = CountryMapper.GetCountriesByRegion("Europe");

// Get all countries in a subregion
List<Country> caribbeanCountries = CountryMapper.GetCountriesBySubregion("Caribbean");

// List all regions
List<string> allRegions = CountryMapper.GetAllRegions();

// List all subregions
List<string> allSubregions = CountryMapper.GetAllSubregions();
```

### Custom Data Sources

```csharp
// Load country data from a JSON file
List<Country> customCountries = CountryDataLoader.LoadCountriesFromJson("countries.json");

// Update the in-memory database
CountryMapper.UpdateCountryDatabase(customCountries);

// Save modified data back to JSON
CountryDataLoader.SaveCountriesToJson(customCountries, "updated-countries.json");
```

## ASP.NET Core Integration

```csharp
// In Startup.cs or Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Register necessary services
    services.AddControllers();
    
    // Load custom country data if needed
    var jsonPath = Path.Combine(env.ContentRootPath, "Data", "countries.json");
    if (File.Exists(jsonPath))
    {
        try 
        {
            var countries = CountryDataLoader.LoadCountriesFromJson(jsonPath);
            CountryMapper.UpdateCountryDatabase(countries);
            logger.LogInformation("Custom country data loaded successfully");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load custom country data, using built-in data");
        }
    }
}
```

## Performance Considerations

For optimal performance:

- **Specify input format** when known using the overloaded methods
- Use **batch operations** for multiple conversions
- The country database is **loaded lazily** on first access
- Consider custom data loading at application startup instead of runtime

## Contributing

Contributions are welcome! Here's how you can contribute:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

Please make sure to update tests and documentation as appropriate.

### Development Setup

```bash
# Clone the repository
git clone https://github.com/yourusername/CountryCodeKit.git

# Navigate to the project directory
cd CountryCodeKit

# Build the solution
dotnet build

# Run the tests
dotnet test
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- ISO 3166-1 standard for country codes
- RegionInfo class in .NET for localized country names

---

Made with ❤️ by Chizaruu