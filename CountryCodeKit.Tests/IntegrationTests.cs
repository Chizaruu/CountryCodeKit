using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CountryCodeKit.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        //[Test]
        //public void UpdateCountryDatabase_ShouldReplaceExistingDatabase()
        //{
        //    // Arrange - Create a custom country database
        //    var customCountries = new List<Country>
        //    {
        //        new Country
        //        {
        //            Name = "Custom Country",
        //            OfficialName = "Republic of Custom Country",
        //            Alpha2Code = "CC",
        //            Alpha3Code = "CCC",
        //            NumericCode = "999",
        //            CallingCode = "999",
        //            Region = "Custom Region",
        //            Subregion = "Custom Subregion"
        //        }
        //    };

        //    // Keep a reference to the original countries
        //    var originalCountries = CountryMapper.Countries.ToList();
        //    try
        //    {
        //        // Act - Update database with custom countries
        //        CountryMapper.UpdateCountryDatabase(customCountries);

        //        // Assert - Verify database was updated
        //        Assert.That(CountryMapper.Countries.Count, Is.EqualTo(1));
        //        Assert.That(CountryMapper.Countries[0].Name, Is.EqualTo("Custom Country"));
        //        Assert.That(CountryMapper.Countries[0].Alpha2Code, Is.EqualTo("CC"));

        //        // Verify conversion now works with custom data
        //        var result = CountryMapper.Convert("CC", CountryCodeFormat.Name);
        //        Assert.That(result, Is.EqualTo("Custom Country"));

        //        var notFound = CountryMapper.Convert("US", CountryCodeFormat.Name);
        //        Assert.IsNull(notFound); // Original country shouldn't be found
        //    }
        //    finally
        //    {
        //        // Restore original database
        //        CountryMapper.UpdateCountryDatabase(originalCountries);
        //    }
        //}

        [Test]
        public void UpdateCountryDatabase_WithEmptyList_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => CountryMapper.UpdateCountryDatabase(new List<Country>()));
        }

        [Test]
        public void ExoticInputsAndCaseInsensitivity_ShouldWork()
        {
            // Act & Assert - Case insensitivity
            var result1 = CountryMapper.Convert("us", CountryCodeFormat.Name);
            var result2 = CountryMapper.Convert("uS", CountryCodeFormat.Name);
            var result3 = CountryMapper.Convert("UNITED STATES", CountryCodeFormat.Alpha2);

            Assert.That(result1, Is.EqualTo("United States"));
            Assert.That(result2, Is.EqualTo("United States"));
            Assert.That(result3, Is.EqualTo("US"));

            // Act & Assert - Whitespace handling
            var result4 = CountryMapper.Convert("  GB  ", CountryCodeFormat.Name);
            Assert.That(result4, Is.EqualTo("United Kingdom"));

            // Act & Assert - Alternative names
            var result5 = CountryMapper.Convert("UK", CountryCodeFormat.Name);
            Assert.That(result5, Is.EqualTo("United Kingdom"));
        }

        [Test]
        public void E2E_JsonRoundTrip_ShouldPreserveData()
        {
            // Arrange
            var testFilePath = Path.Combine(Path.GetTempPath(), "e2e-test.json");
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            try
            {
                // Get a subset of countries to work with
                var originalCountries = CountryMapper.Countries.Take(5).ToList();

                // Act - Save and reload
                CountryDataLoader.SaveCountriesToJson(originalCountries, testFilePath);
                var loadedCountries = CountryDataLoader.LoadCountriesFromJson(testFilePath);

                // Assert - Check counts match
                Assert.That(loadedCountries.Count, Is.EqualTo(originalCountries.Count));

                // Check all countries were preserved
                foreach (var original in originalCountries)
                {
                    var loaded = loadedCountries.FirstOrDefault(c => c.Alpha2Code == original.Alpha2Code);
                    Assert.That(loaded, Is.Not.Null);
                    Assert.That(loaded.Name, Is.EqualTo(original.Name));
                    Assert.That(loaded.Alpha3Code, Is.EqualTo(original.Alpha3Code));
                    Assert.That(loaded.NumericCode, Is.EqualTo(original.NumericCode));
                }
            }
            finally
            {
                // Clean up
                if (File.Exists(testFilePath))
                    File.Delete(testFilePath);
            }
        }

        //    [Test]
        //    public void CountryConversionMatrix_ShouldWorkForAllFormats()
        //    {
        //        // Arrange - Get a real country to test with
        //        var usa = CountryMapper.FindCountry("US");
        //        Assert.That(usa, Is.Not.Null);

        //        // Enum all formats
        //        var formats = Enum.GetValues<CountryCodeFormat>();

        //        // Act & Assert - Test conversion between all format pairs
        //        foreach (var inputFormat in formats)
        //        {
        //            // Get input value
        //            var inputValue = CountryMapper.GetCountryFormat(usa, inputFormat);

        //            // Skip if this format doesn't have a value (null or empty)
        //            if (string.IsNullOrEmpty(inputValue)) continue;

        //            foreach (var outputFormat in formats)
        //            {
        //                // Get expected output directly from country
        //                var expectedOutput = CountryMapper.GetCountryFormat(usa, outputFormat);

        //                // Convert using the mapper
        //                var actualOutput = CountryMapper.Convert(inputValue, inputFormat, outputFormat);

        //                // Assert values match
        //                Assert.That(actualOutput, Is.EqualTo(expectedOutput));
        //            }
        //        }
        //    }
    }
}