using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CountryCodeKit.Tests
{
    [TestFixture]
    public class CountryDataLoaderTests
    {
        private string _validJsonPath;
        private string _invalidJsonPath;
        private string _emptyJsonPath;

        [SetUp]
        public void Setup()
        {
            // Set up test files
            _validJsonPath = Path.Combine(Path.GetTempPath(), "valid-countries.json");
            _invalidJsonPath = Path.Combine(Path.GetTempPath(), "invalid-countries.json");
            _emptyJsonPath = Path.Combine(Path.GetTempPath(), "empty-countries.json");

            // Create a valid JSON file for testing
            string validJson = @"[
                {
                    ""name"": ""Test Country"",
                    ""officialName"": ""The Test Country"",
                    ""alpha2Code"": ""TC"",
                    ""alpha3Code"": ""TCO"",
                    ""numericCode"": ""999"",
                    ""callingCode"": ""999"",
                    ""region"": ""Test Region"",
                    ""subregion"": ""Test Subregion"",
                    ""alternativeNames"": [""TestLand""],
                    ""localizedNames"": {
                        ""es"": ""País de Prueba"",
                        ""fr"": ""Pays de Test""
                    }
                }
            ]";

            // Create an invalid JSON file
            string invalidJson = @"{""this"": ""is not valid JSON for countries";

            // Create empty JSON array
            string emptyJson = "[]";

            // Write test files
            File.WriteAllText(_validJsonPath, validJson);
            File.WriteAllText(_invalidJsonPath, invalidJson);
            File.WriteAllText(_emptyJsonPath, emptyJson);
        }

        [TearDown]
        public void Cleanup()
        {
            // Clean up test files
            if (File.Exists(_validJsonPath))
                File.Delete(_validJsonPath);

            if (File.Exists(_invalidJsonPath))
                File.Delete(_invalidJsonPath);

            if (File.Exists(_emptyJsonPath))
                File.Delete(_emptyJsonPath);
        }

        [Test]
        public void LoadCountriesFromJson_WithValidFile_ShouldReturnCountries()
        {
            // Act
            var countries = CountryDataLoader.LoadCountriesFromJson(_validJsonPath);

            // Assert
            Assert.That(countries, Is.Not.Null);
            Assert.That(countries.Count, Is.EqualTo(1));

            var country = countries.First();
            Assert.That(country.Name, Is.EqualTo("Test Country"));
            Assert.That(country.OfficialName, Is.EqualTo("The Test Country"));
            Assert.That(country.Alpha2Code, Is.EqualTo("TC"));
            Assert.That(country.Alpha3Code, Is.EqualTo("TCO"));
            Assert.That(country.NumericCode, Is.EqualTo("999"));
            Assert.That(country.CallingCode, Is.EqualTo("999"));
            Assert.That(country.Region, Is.EqualTo("Test Region"));
            Assert.That(country.Subregion, Is.EqualTo("Test Subregion"));
            CollectionAssert.Contains(country.AlternativeNames, "TestLand");

            // Check localized names
            Assert.That(country.LocalizedNames.Count, Is.EqualTo(2));
            Assert.That(country.LocalizedNames["es"], Is.EqualTo("País de Prueba"));
            Assert.That(country.LocalizedNames["fr"], Is.EqualTo("Pays de Test"));
        }

        [Test]
        public void LoadCountriesFromJson_WithEmptyFile_ShouldReturnEmptyList()
        {
            // Act
            var countries = CountryDataLoader.LoadCountriesFromJson(_emptyJsonPath);

            // Assert
            Assert.That(countries, Is.Not.Null);
            Assert.IsEmpty(countries);
        }

        [Test]
        public void LoadCountriesFromJson_WithInvalidJson_ShouldThrowFormatException()
        {
            // Act & Assert
            var exception = Assert.Throws<FormatException>(() =>
                CountryDataLoader.LoadCountriesFromJson(_invalidJsonPath));

            StringAssert.Contains("Error parsing country data JSON", exception.Message);
        }

        [Test]
        public void LoadCountriesFromJson_WithNonExistentFile_ShouldThrowFileNotFoundException()
        {
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() =>
                CountryDataLoader.LoadCountriesFromJson("non-existent-file.json"));
        }

        [Test]
        public void LoadCountriesFromJson_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CountryDataLoader.LoadCountriesFromJson(null));
        }

        [Test]
        public void SaveCountriesToJson_ShouldCreateValidFile()
        {
            // Arrange
            var testFilePath = Path.Combine(Path.GetTempPath(), "save-test.json");
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            var countries = new List<Country>
            {
                new Country
                {
                    Name = "Saved Country",
                    OfficialName = "Republic of Saved Country",
                    Alpha2Code = "SC",
                    Alpha3Code = "SCO",
                    NumericCode = "888",
                    CallingCode = "888",
                    Region = "Save Region",
                    Subregion = "Save Subregion",
                    AlternativeNames = new List<string> { "SaveLand" },
                    LocalizedNames = new Dictionary<string, string>
                    {
                        { "es", "País Guardado" },
                        { "fr", "Pays Sauvegardé" }
                    }
                }
            };

            // Act
            CountryDataLoader.SaveCountriesToJson(countries, testFilePath);

            // Assert
            Assert.IsTrue(File.Exists(testFilePath));

            // Attempt to load it back to verify it's valid
            var loadedCountries = CountryDataLoader.LoadCountriesFromJson(testFilePath);
            Assert.That(loadedCountries, Is.Not.Null);
            Assert.That(loadedCountries.Count, Is.EqualTo(1));

            var country = loadedCountries.First();
            Assert.That(country.Name, Is.EqualTo("Saved Country"));
            Assert.That(country.Alpha2Code, Is.EqualTo("SC"));
            Assert.That(country.Alpha3Code, Is.EqualTo("SCO"));

            // Clean up
            File.Delete(testFilePath);
        }

        [Test]
        public void SaveCountriesToJson_WithNullCountries_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                CountryDataLoader.SaveCountriesToJson(null, "test.json"));
        }

        [Test]
        public void SaveCountriesToJson_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                CountryDataLoader.SaveCountriesToJson(new List<Country>(), null));
        }
    }
}