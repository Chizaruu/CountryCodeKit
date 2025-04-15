using System.Text.Json;

namespace CountryCodeKit
{
    /// <summary>
    /// Provides methods for loading country data from external JSON files
    /// </summary>
    public static class CountryDataLoader
    {
        /// <summary>
        /// Loads country data from a JSON file
        /// </summary>
        /// <param name="jsonFilePath">Path to the JSON file containing country data</param>
        /// <returns>A list of Country objects</returns>
        public static List<Country> LoadCountriesFromJson(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentNullException(nameof(jsonFilePath));

            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException($"Country data file not found: {jsonFilePath}");

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var countries = JsonSerializer.Deserialize<List<Country>>(jsonContent, options);
                return countries ?? [];
            }
            catch (JsonException ex)
            {
                throw new FormatException($"Error parsing country data JSON: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading country data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads country data from an embedded resource JSON file
        /// </summary>
        /// <param name="resourceName">The name of the embedded resource</param>
        /// <returns>A list of Country objects</returns>
        public static List<Country> LoadCountriesFromEmbeddedResource(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            try
            {
                // Get the current assembly
                var assembly = typeof(CountryDataLoader).Assembly;

                // Open the resource stream
                using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

                // Read the stream
                using var reader = new StreamReader(stream);
                string jsonContent = reader.ReadToEnd();

                // Deserialize the JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var countries = JsonSerializer.Deserialize<List<Country>>(jsonContent, options);
                return countries ?? [];
            }
            catch (JsonException ex)
            {
                throw new FormatException($"Error parsing country data JSON: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading country data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Saves a list of Country objects to a JSON file
        /// </summary>
        /// <param name="countries">The list of countries to save</param>
        /// <param name="jsonFilePath">The file path to save to</param>
        public static void SaveCountriesToJson(List<Country> countries, string jsonFilePath)
        {
            ArgumentNullException.ThrowIfNull(countries);

            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentNullException(nameof(jsonFilePath));

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(countries, options);
                File.WriteAllText(jsonFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving country data: {ex.Message}", ex);
            }
        }
    }
}