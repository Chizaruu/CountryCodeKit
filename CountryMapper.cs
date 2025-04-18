using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;

namespace CountryCodeKit
{
    /// <summary>
    /// Provides methods for converting between different country code formats
    /// </summary>
    public static partial class CountryMapper
    {
        private static readonly Lazy<List<Country>> _countries = new(InitializeCountryData);

        /// <summary>
        /// Gets the list of all countries
        /// </summary>
        public static List<Country> Countries => _countries.Value;

        /// <summary>
        /// Initializes the country data
        /// </summary>
        private static List<Country> InitializeCountryData()
        {
            try
            {
                // Configure JSON serializer options to handle property name casing
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                // Try to load from embedded resource first
                var embeddedResourceName = typeof(CountryMapper).Namespace + ".Data.countries.json";
                var stream = typeof(CountryMapper).Assembly.GetManifestResourceStream(embeddedResourceName);

                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var json = reader.ReadToEnd();
                    var countries = JsonSerializer.Deserialize<List<Country>>(json, jsonOptions);

                    if (countries != null && countries.Count > 0)
                    {
                        return countries;
                    }
                }

                // Fallback to loading from file if available
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "countries.json");
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var countries = JsonSerializer.Deserialize<List<Country>>(json, jsonOptions);

                    if (countries != null && countries.Count > 0)
                    {
                        return countries;
                    }
                }
            }
            catch
            {
                // Silently continue to empty list
            }

            // If all attempts failed, throw an exception
            throw new InvalidOperationException("Country data not found. Please provide a valid JSON file or embedded resource.");
        }

        /// <summary>
        /// Converts from a country code or name to the desired format
        /// </summary>
        /// <param name="input">The input string (country name, code, etc.)</param>
        /// <param name="outputFormat">The desired output format</param>
        /// <param name="cultureName">The culture for localized output (optional)</param>
        /// <returns>The country code or name in the desired format, or null if not found</returns>
        public static string? Convert(string input, CountryCodeFormat outputFormat, string cultureName = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Clean input
            input = input.Trim();

            // Special case for numeric codes - try direct conversion first
            if (int.TryParse(input, out int numericCode))
            {
                var countryByNumeric = Countries.FirstOrDefault(c =>
                    int.TryParse(c.NumericCode, out int countryNumeric) && countryNumeric == numericCode);

                if (countryByNumeric != null)
                    return GetCountryFormat(countryByNumeric, outputFormat, cultureName);
            }

            // Try to find the country using normal methods
            var country = FindCountry(input);
            if (country == null)
                return null;

            // Return the requested format
            return GetCountryFormat(country, outputFormat, cultureName);
        }

        /// <summary>
        /// Converts from a country code or name to the desired format with a known input format for improved performance
        /// </summary>
        /// <param name="input">The input string (country name, code, etc.)</param>
        /// <param name="inputFormat">The format of the input string</param>
        /// <param name="outputFormat">The desired output format</param>
        /// <param name="cultureName">The culture for localized output (optional)</param>
        /// <returns>The country code or name in the desired format, or null if not found</returns>
        public static string? Convert(string input, CountryCodeFormat inputFormat, CountryCodeFormat outputFormat, string cultureName = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Clean input
            input = input.Trim();

            // Find country based on input format
            Country? country = null;
            switch (inputFormat)
            {
                case CountryCodeFormat.Name:
                    country = Countries.FirstOrDefault(c => string.Equals(c.Name, input, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.OfficialName:
                    country = Countries.FirstOrDefault(c => string.Equals(c.OfficialName, input, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.Alpha2:
                    country = Countries.FirstOrDefault(c => string.Equals(c.Alpha2Code, input, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.Alpha3:
                    country = Countries.FirstOrDefault(c => string.Equals(c.Alpha3Code, input, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.Numeric:
                    country = Countries.FirstOrDefault(c => string.Equals(c.NumericCode, input, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.CallingCode:
                    country = Countries.FirstOrDefault(c => string.Equals(c.CallingCode, input, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.CallingCodeWithPlus:
                    string callingCode = input.StartsWith('+') ? input[1..] : input;
                    country = Countries.FirstOrDefault(c => string.Equals(c.CallingCode, callingCode, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.LocalizedName:
                    // This is trickier since we need to search through localized names
                    country = Countries.FirstOrDefault(c =>
                        c.LocalizedNames.Any(n => string.Equals(n.Value, input, StringComparison.OrdinalIgnoreCase)));
                    break;
                case CountryCodeFormat.Region:
                    // Return all countries in the specified region
                    country = Countries.FirstOrDefault(c => string.Equals(c.Region, input, StringComparison.OrdinalIgnoreCase));
                    break;
                case CountryCodeFormat.Subregion:
                    // Return all countries in the specified subregion
                    country = Countries.FirstOrDefault(c => string.Equals(c.Subregion, input, StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    // If unknown format, try the general approach
                    country = FindCountry(input);
                    break;
            }

            if (country == null)
                return null;

            // Return the requested format
            return GetCountryFormat(country, outputFormat, cultureName);
        }

        /// <summary>
        /// Finds a country by any of its identifiers
        /// </summary>
        /// <param name="input">The input string to search for</param>
        /// <returns>The matching Country object, or null if not found</returns>
        public static Country? FindCountry(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            // Try exact matches first
            var country = Countries.FirstOrDefault(c =>
                string.Equals(c.Name, input, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.OfficialName, input, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Alpha2Code, input, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Alpha3Code, input, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.NumericCode, input, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.CallingCode, input.TrimStart('+'), StringComparison.OrdinalIgnoreCase) ||
                c.AlternativeNames.Any(n => string.Equals(n, input, StringComparison.OrdinalIgnoreCase)) ||
                c.LocalizedNames.Any(n => string.Equals(n.Value, input, StringComparison.OrdinalIgnoreCase))
            );

            if (country != null)
                return country;

            // Try numeric code comparison
            if (int.TryParse(input, out int numericCode))
            {
                country = Countries.FirstOrDefault(c =>
                    int.TryParse(c.NumericCode, out int countryNumeric) && countryNumeric == numericCode);

                if (country != null)
                    return country;
            }

            // Handle phone code format (with or without + prefix)
            string phoneCode = input;
            if (input.StartsWith('+'))
            {
                // Simple phone code (like "+1" or "+44")
                phoneCode = input.TrimStart('+');

                // Try direct match first (for simple codes like "+1")
                country = Countries.FirstOrDefault(c =>
                    string.Equals(c.CallingCode, phoneCode, StringComparison.OrdinalIgnoreCase));

                if (country != null)
                    return country;

                // For longer phone numbers, try to extract just the country code
                if (input.Length > 2)
                {
                    var match = PhoneRegex().Match(input);
                    if (match.Success)
                    {
                        phoneCode = match.Groups[1].Value;
                        country = Countries.FirstOrDefault(c =>
                            string.Equals(c.CallingCode, phoneCode, StringComparison.OrdinalIgnoreCase));

                        if (country != null)
                            return country;
                    }
                }
            }
            else
            {
                // Try to match without + prefix (e.g., "1" or "44")
                country = Countries.FirstOrDefault(c =>
                    string.Equals(c.CallingCode, input, StringComparison.OrdinalIgnoreCase));

                if (country != null)
                    return country;
            }

            // Try to find by partial name match if no exact match found
            country = Countries.FirstOrDefault(c =>
                (c.Name != null && c.Name.Contains(input, StringComparison.OrdinalIgnoreCase)) ||
                (c.OfficialName != null && c.OfficialName.Contains(input, StringComparison.OrdinalIgnoreCase)) ||
                (c.AlternativeNames != null && c.AlternativeNames.Any(n => n != null && n.Contains(input, StringComparison.OrdinalIgnoreCase))) ||
                (c.LocalizedNames != null && c.LocalizedNames.Any(n => n.Value != null && n.Value.Contains(input, StringComparison.OrdinalIgnoreCase)))
            );

            return country;
        }

        /// <summary>
        /// Gets the specified format for a country
        /// </summary>
        /// <param name="country">The Country object</param>
        /// <param name="format">The desired output format</param>
        /// <param name="cultureName">The culture name for localized output (optional)</param>
        /// <returns>The requested country format as a string</returns>
        public static string? GetCountryFormat(Country country, CountryCodeFormat format, string cultureName = null)
        {
            if (country == null)
                return null;

            switch (format)
            {
                case CountryCodeFormat.Name:
                    return country.Name;

                case CountryCodeFormat.OfficialName:
                    return country.OfficialName;

                case CountryCodeFormat.Alpha2:
                    return country.Alpha2Code;

                case CountryCodeFormat.Alpha3:
                    return country.Alpha3Code;

                case CountryCodeFormat.Numeric:
                    return country.NumericCode;

                case CountryCodeFormat.CallingCode:
                    return country.CallingCode;

                case CountryCodeFormat.CallingCodeWithPlus:
                    return "+" + country.CallingCode;

                case CountryCodeFormat.LocalizedName:
                    if (!string.IsNullOrEmpty(cultureName) && country.LocalizedNames.TryGetValue(cultureName, out string? value))
                        return value;

                    // If the requested culture is not found, try to get it from the .NET cultures
                    try
                    {
                        var cultureInfo = string.IsNullOrEmpty(cultureName)
                            ? CultureInfo.CurrentCulture
                            : new CultureInfo(cultureName);

                        var regionInfo = new RegionInfo(country.Alpha2Code);
                        return regionInfo.DisplayName;
                    }
                    catch
                    {
                        // If we can't get the localized name, return the English name
                        return country.Name;
                    }

                case CountryCodeFormat.Region:
                    return country.Region;

                case CountryCodeFormat.Subregion:
                    return country.Subregion;

                default:
                    return country.Name;
            }
        }

        /// <summary>
        /// Batch converts a list of country identifiers to the desired format
        /// </summary>
        /// <param name="inputs">The input strings (country names, codes, etc.)</param>
        /// <param name="outputFormat">The desired output format</param>
        /// <param name="cultureName">The culture for localized output (optional)</param>
        /// <returns>A dictionary mapping the input to the converted output</returns>
        public static Dictionary<string, string> BatchConvert(
            IEnumerable<string> inputs,
            CountryCodeFormat outputFormat,
            string? cultureName = null)
        {
            var result = new Dictionary<string, string>();
            foreach (var input in inputs)
            {
                result[input] = Convert(input, outputFormat, cultureName);
            }
            return result;
        }

        /// <summary>
        /// Batch converts a list of country identifiers to the desired format with a known input format for improved performance
        /// </summary>
        /// <param name="inputs">The input strings (country names, codes, etc.)</param>
        /// <param name="inputFormat">The format of the input strings</param>
        /// <param name="outputFormat">The desired output format</param>
        /// <param name="cultureName">The culture for localized output (optional)</param>
        /// <returns>A dictionary mapping the input to the converted output</returns>
        public static Dictionary<string, string> BatchConvert(
            IEnumerable<string> inputs,
            CountryCodeFormat inputFormat,
            CountryCodeFormat outputFormat,
            string? cultureName = null)
        {
            var result = new Dictionary<string, string>();
            foreach (var input in inputs)
            {
                result[input] = Convert(input, inputFormat, outputFormat, cultureName);
            }
            return result;
        }

        /// <summary>
        /// Checks if a given string is a valid country code or name
        /// </summary>
        /// <param name="input">The input string to validate</param>
        /// <returns>True if the input is a valid country identifier, false otherwise</returns>
        public static bool IsValidCountry(string input) => FindCountry(input) != null;

        /// <summary>
        /// Gets all countries in a specific region
        /// </summary>
        /// <param name="region">The region name</param>
        /// <returns>A list of countries in the specified region</returns>
        public static List<Country> GetCountriesByRegion(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                return [];

            return Countries
                .Where(c => string.Equals(c.Region, region, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Gets all countries in a specific subregion
        /// </summary>
        /// <param name="subregion">The subregion name</param>
        /// <returns>A list of countries in the specified subregion</returns>
        public static List<Country> GetCountriesBySubregion(string subregion)
        {
            if (string.IsNullOrWhiteSpace(subregion))
                return [];

            return Countries
                .Where(c => string.Equals(c.Subregion, subregion, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Gets all available regions
        /// </summary>
        /// <returns>A list of all unique region names</returns>
        public static List<string> GetAllRegions() => [.. Countries
                .Select(c => c.Region)
                .Distinct()
                .OrderBy(r => r)];

        /// <summary>
        /// Gets all available subregions
        /// </summary>
        /// <returns>A list of all unique subregion names</returns>
        public static List<string> GetAllSubregions() => [.. Countries
                .Select(c => c.Subregion)
                .Distinct()
                .OrderBy(r => r)];

        /// <summary>
        /// Updates the country database with custom country data
        /// </summary>
        /// <param name="countries">The list of countries to use</param>
        public static void UpdateCountryDatabase(List<Country> countries)
        {
            if (countries == null || countries.Count == 0)
                throw new ArgumentException("Country list cannot be null or empty", nameof(countries));

            // Use reflection to update the private _countries field
            var field = typeof(CountryMapper).GetField("_countries",
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Static);

            // Create a new Lazy<List<Country>> with the provided countries
            var newLazy = new Lazy<List<Country>>(() => countries);

            // Set the field value to the new lazy instance
            field?.SetValue(null, newLazy);
        }

        [GeneratedRegex(@"^\+(\d{1,3})")]
        private static partial Regex PhoneRegex();
    }
}