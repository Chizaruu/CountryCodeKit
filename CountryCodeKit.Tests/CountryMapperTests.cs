using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CountryCodeKit.Tests
{
    [TestFixture]
    public class CountryMapperTests
    {
        [Test]
        public void Countries_ShouldNotBeEmpty()
        {
            // Act
            var countries = CountryMapper.Countries;

            // Assert
            Assert.That(countries, Is.Not.Null);
            Assert.IsNotEmpty(countries);
        }

        [Test]
        [TestCase("US", CountryCodeFormat.Name, "United States")]
        [TestCase("GB", CountryCodeFormat.Name, "United Kingdom")]
        [TestCase("United States", CountryCodeFormat.Alpha2, "US")]
        [TestCase("United Kingdom", CountryCodeFormat.Alpha3, "GBR")]
        [TestCase("840", CountryCodeFormat.Alpha2, "US")]
        [TestCase("826", CountryCodeFormat.Alpha3, "GBR")]
        [TestCase("CA", CountryCodeFormat.OfficialName, "Canada")]
        [TestCase("JP", CountryCodeFormat.CallingCode, "81")]
        [TestCase("BR", CountryCodeFormat.CallingCodeWithPlus, "+55")]
        public void Convert_ShouldReturnCorrectValue(string input, CountryCodeFormat outputFormat, string expectedOutput)
        {
            // Act
            var result = CountryMapper.Convert(input, outputFormat);

            // Assert
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase("US", CountryCodeFormat.Alpha2, CountryCodeFormat.Name, "United States")]
        [TestCase("GBR", CountryCodeFormat.Alpha3, CountryCodeFormat.Alpha2, "GB")]
        [TestCase("840", CountryCodeFormat.Numeric, CountryCodeFormat.Alpha3, "USA")]
        [TestCase("United States", CountryCodeFormat.Name, CountryCodeFormat.OfficialName, "United States of America")]
        public void Convert_WithKnownInputFormat_ShouldReturnCorrectValue(
            string input,
            CountryCodeFormat inputFormat,
            CountryCodeFormat outputFormat,
            string expectedOutput)
        {
            // Act
            var result = CountryMapper.Convert(input, inputFormat, outputFormat);

            // Assert
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("XX")]
        [TestCase("INVALID")]
        public void Convert_WithInvalidInput_ShouldReturnNull(string input)
        {
            // Act
            var result = CountryMapper.Convert(input, CountryCodeFormat.Name);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void FindCountry_WithValidInputs_ShouldReturnMatchingCountry()
        {
            // Act & Assert - Alpha2
            var countryByAlpha2 = CountryMapper.FindCountry("US");
            Assert.That(countryByAlpha2, Is.Not.Null);
            Assert.That(countryByAlpha2.Name, Is.EqualTo("United States"));

            // Act & Assert - Alpha3
            var countryByAlpha3 = CountryMapper.FindCountry("CAN");
            Assert.That(countryByAlpha3, Is.Not.Null);
            Assert.That(countryByAlpha3.Name, Is.EqualTo("Canada"));

            // Act & Assert - Name
            var countryByName = CountryMapper.FindCountry("United Kingdom");
            Assert.That(countryByName, Is.Not.Null);
            Assert.That(countryByName.Alpha2Code, Is.EqualTo("GB"));

            // Act & Assert - Numeric
            var countryByNumeric = CountryMapper.FindCountry("392");
            Assert.That(countryByNumeric, Is.Not.Null);
            Assert.That(countryByNumeric.Name, Is.EqualTo("Japan"));

            // Act & Assert - Alternative name
            var countryByAlt = CountryMapper.FindCountry("UK");
            Assert.That(countryByAlt, Is.Not.Null);
            Assert.That(countryByAlt.Name, Is.EqualTo("United Kingdom"));

            // Act & Assert - Partial match
            //var countryByPartial = CountryMapper.FindCountry("States");
            //Assert.That(countryByPartial, Is.Not.Null);
            //Assert.That(countryByPartial.Name, Is.EqualTo("United States"));
        }

        [Test]
        public void FindCountry_WithPhoneCode_ShouldReturnCorrectCountry()
        {
            // Act - With + prefix
            var countryByPhone1 = CountryMapper.FindCountry("+1");

            // Assert - Note: +1 is shared by US and Canada, but US is usually first in the list
            Assert.That(countryByPhone1, Is.Not.Null);
            Assert.That(countryByPhone1.CallingCode, Is.EqualTo("1"));

            // Act & Assert - Without + prefix
            var countryByPhone2 = CountryMapper.FindCountry("44");
            Assert.That(countryByPhone2, Is.Not.Null);
            Assert.That(countryByPhone2.Name, Is.EqualTo("United Kingdom"));

            //// Act & Assert - Phone with extra digits
            //var countryByPhone3 = CountryMapper.FindCountry("+81123456789");
            //Assert.That(countryByPhone3, Is.Not.Null);
            //Assert.That(countryByPhone3.Name, Is.EqualTo("Japan"));
        }

        [Test]
        public void BatchConvert_ShouldConvertMultipleInputs()
        {
            // Arrange
            var inputs = new List<string> { "US", "GB", "CA" };

            // Act
            var results = CountryMapper.BatchConvert(inputs, CountryCodeFormat.Name);

            // Assert
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results["US"], Is.EqualTo("United States"));
            Assert.That(results["GB"], Is.EqualTo("United Kingdom"));
            Assert.That(results["CA"], Is.EqualTo("Canada"));
        }

        [Test]
        public void BatchConvert_WithKnownInputFormat_ShouldConvertFaster()
        {
            // Arrange
            var inputs = new List<string> { "US", "GB", "CA" };

            // Act
            var results = CountryMapper.BatchConvert(inputs, CountryCodeFormat.Alpha2, CountryCodeFormat.Alpha3);

            // Assert
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results["US"], Is.EqualTo("USA"));
            Assert.That(results["GB"], Is.EqualTo("GBR"));
            Assert.That(results["CA"], Is.EqualTo("CAN"));
        }

        [Test]
        [TestCase("US", true)]
        [TestCase("USA", true)]
        [TestCase("United States", true)]
        [TestCase("America", true)]
        [TestCase("XX", false)]
        [TestCase("INVALID", false)]
        [TestCase("", false)]
        public void IsValidCountry_ShouldReturnCorrectResult(string input, bool expected)
        {
            // Act
            var result = CountryMapper.IsValidCountry(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetCountriesByRegion_ShouldReturnCountriesInRegion()
        {
            // Act
            var europeanCountries = CountryMapper.GetCountriesByRegion("Europe");

            // Assert
            Assert.IsNotEmpty(europeanCountries);

            // Replace CollectionAssert.All with a foreach loop or LINQ
            foreach (var country in europeanCountries)
            {
                Assert.That(country.Region, Is.EqualTo("Europe"));
            }

            // Verify specific countries are in the list
            Assert.IsTrue(europeanCountries.Any(c => c.Alpha2Code == "GB"));
            Assert.IsTrue(europeanCountries.Any(c => c.Alpha2Code == "FR"));
        }

        [Test]
        public void GetCountriesBySubregion_ShouldReturnCountriesInSubregion()
        {
            // Act
            var northernEuropeCountries = CountryMapper.GetCountriesBySubregion("Northern Europe");

            // Assert
            Assert.IsNotEmpty(northernEuropeCountries);

            // Check all countries have the correct subregion
            foreach (var country in northernEuropeCountries)
            {
                Assert.That(country.Subregion, Is.EqualTo("Northern Europe"));
            }

            Assert.IsTrue(northernEuropeCountries.Any(c => c.Alpha2Code == "GB"));
        }

        [Test]
        public void GetAllRegions_ShouldReturnUniqueRegions()
        {
            // Act
            var regions = CountryMapper.GetAllRegions();

            // Assert
            Assert.IsNotEmpty(regions);
            CollectionAssert.Contains(regions, "Europe");
            CollectionAssert.Contains(regions, "Asia");
            CollectionAssert.Contains(regions, "Americas");
            CollectionAssert.Contains(regions, "Africa");
            CollectionAssert.Contains(regions, "Oceania");

            // Ensure uniqueness
            Assert.That(regions.Distinct().Count(), Is.EqualTo(regions.Count));
        }

        [Test]
        public void GetAllSubregions_ShouldReturnUniqueSubregions()
        {
            // Act
            var subregions = CountryMapper.GetAllSubregions();

            // Assert
            Assert.IsNotEmpty(subregions);
            CollectionAssert.Contains(subregions, "Northern Europe");
            CollectionAssert.Contains(subregions, "Southern Europe");
            CollectionAssert.Contains(subregions, "Western Europe");
            CollectionAssert.Contains(subregions, "Eastern Europe");

            // Ensure uniqueness
            Assert.That(subregions.Distinct().Count(), Is.EqualTo(subregions.Count));
        }

        [Test]
        public void GetCountryFormat_WithLocalization_ShouldReturnLocalizedNames()
        {
            // Arrange
            var usa = CountryMapper.FindCountry("US");
            Assert.That(usa, Is.Not.Null);

            // Act - Spanish
            var spanishName = CountryMapper.GetCountryFormat(usa, CountryCodeFormat.LocalizedName, "es");

            // Assert
            Assert.That(spanishName, Is.EqualTo("Estados Unidos"));

            // Act - French
            var frenchName = CountryMapper.GetCountryFormat(usa, CountryCodeFormat.LocalizedName, "fr");

            // Assert
            Assert.That(frenchName, Is.EqualTo("États-Unis"));
        }
    }
}