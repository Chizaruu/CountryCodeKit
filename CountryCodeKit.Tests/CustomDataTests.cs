using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace CountryCodeKit.Tests
{
    [TestFixture]
    public class CustomDataTests
    {
        private readonly string _sampleJsonResourceName = "CountryCodeKit.Tests.TestData.sample-countries.json";

        [Test]
        public void LoadCountriesFromEmbeddedResource_ShouldLoadTestData()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            // Ensure our test resource exists
            CollectionAssert.Contains(resourceNames, _sampleJsonResourceName);

            // Act
            var stream = assembly.GetManifestResourceStream(_sampleJsonResourceName);
            Assert.That(stream, Is.Not.Null);

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            // Assert
            Assert.IsNotEmpty(json);
            StringAssert.Contains("Test Country Alpha", json);
            StringAssert.Contains("Test Country Beta", json);
            StringAssert.Contains("Test Country Gamma", json);
        }

        [Test]
        public void FindCountry_WithCustomDatabase_ShouldFindCustomCountries()
        {
            // Arrange - Create custom countries
            var customCountries = new List<Country>
            {
                new Country
                {
                    Name = "Custom Country",
                    OfficialName = "Republic of Custom",
                    Alpha2Code = "CC",
                    Alpha3Code = "CCC",
                    NumericCode = "999",
                    CallingCode = "999",
                    Region = "Custom Region",
                    Subregion = "Custom Subregion",
                    AlternativeNames = new List<string> { "CustomLand" },
                    LocalizedNames = new Dictionary<string, string>
                    {
                        { "es", "País Personalizado" },
                        { "fr", "Pays Personnalisé" }
                    }
                },
                new Country
                {
                    Name = "Another Custom",
                    OfficialName = "Federation of Another Custom",
                    Alpha2Code = "AC",
                    Alpha3Code = "ACC",
                    NumericCode = "998",
                    CallingCode = "998",
                    Region = "Custom Region",
                    Subregion = "Another Subregion",
                    AlternativeNames = new List<string> { "AnotherLand" },
                    LocalizedNames = new Dictionary<string, string>
                    {
                        { "es", "Otro País" },
                        { "fr", "Autre Pays" }
                    }
                }
            };

            // Keep original database
            var originalCountries = CountryMapper.Countries.ToList();

            try
            {
                // Act - Update with custom database
                CountryMapper.UpdateCountryDatabase(customCountries);

                // Assert - Find by various properties
                var country1 = CountryMapper.FindCountry("CC");
                Assert.That(country1, Is.Not.Null);
                Assert.That(country1.Name, Is.EqualTo("Custom Country"));

                var country2 = CountryMapper.FindCountry("ACC");
                Assert.That(country2, Is.Not.Null);
                Assert.That(country2.Name, Is.EqualTo("Another Custom"));

                var country3 = CountryMapper.FindCountry("998");
                Assert.That(country3, Is.Not.Null);
                Assert.That(country3.Name, Is.EqualTo("Another Custom"));

                var country4 = CountryMapper.FindCountry("CustomLand");
                Assert.That(country4, Is.Not.Null);
                Assert.That(country4.Name, Is.EqualTo("Custom Country"));

                // Original countries should not be found
                var missing = CountryMapper.FindCountry("US");
                Assert.IsNull(missing);
            }
            finally
            {
                // Restore original database
                CountryMapper.UpdateCountryDatabase(originalCountries);
            }
        }

        [Test]
        public void GetAllRegionsAndSubregions_WithCustomDatabase_ShouldReturnCustomValues()
        {
            // Arrange - Create custom data with unique regions
            var customCountries = new List<Country>
            {
                new Country { Name = "Country1", Alpha2Code = "C1", Region = "Region A", Subregion = "Subregion X" },
                new Country { Name = "Country2", Alpha2Code = "C2", Region = "Region A", Subregion = "Subregion Y" },
                new Country { Name = "Country3", Alpha2Code = "C3", Region = "Region B", Subregion = "Subregion Z" }
            };

            // Keep original database
            var originalCountries = CountryMapper.Countries.ToList();

            try
            {
                // Act - Update with custom database
                CountryMapper.UpdateCountryDatabase(customCountries);

                // Act - Get regions and subregions
                var regions = CountryMapper.GetAllRegions();
                var subregions = CountryMapper.GetAllSubregions();

                // Assert
                Assert.That(regions.Count, Is.EqualTo(2));
                CollectionAssert.Contains(regions, "Region A");
                CollectionAssert.Contains(regions, "Region B");

                Assert.That(subregions.Count, Is.EqualTo(3));
                CollectionAssert.Contains(subregions, "Subregion X");
                CollectionAssert.Contains(subregions, "Subregion Y");
                CollectionAssert.Contains(subregions, "Subregion Z");

                // Test filtering
                var regionACountries = CountryMapper.GetCountriesByRegion("Region A");
                Assert.That(regionACountries.Count, Is.EqualTo(2));
                Assert.IsTrue(regionACountries.Any(c => c.Alpha2Code == "C1"));
                Assert.IsTrue(regionACountries.Any(c => c.Alpha2Code == "C2"));

                var subregionZCountries = CountryMapper.GetCountriesBySubregion("Subregion Z");
                Assert.That(subregionZCountries.Count, Is.EqualTo(1));
                Assert.That(subregionZCountries[0].Alpha2Code, Is.EqualTo("C3"));
            }
            finally
            {
                // Restore original database
                CountryMapper.UpdateCountryDatabase(originalCountries);
            }
        }
    }
}