using NUnit.Framework;
using System;

namespace CountryCodeKit.Tests
{
    [TestFixture]
    public class CountryCodeFormatTests
    {
        [Test]
        public void CountryCodeFormat_ShouldHaveAllRequiredValues()
        {
            // Assert - Check all expected enum values exist
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.Name));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.OfficialName));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.Alpha2));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.Alpha3));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.Numeric));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.CallingCode));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.CallingCodeWithPlus));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.LocalizedName));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.Region));
            Assert.IsTrue(Enum.IsDefined(typeof(CountryCodeFormat), CountryCodeFormat.Subregion));
        }

        [Test]
        public void CountryCodeFormat_ShouldBeEnumWithUniqueValues()
        {
            // Arrange
            var values = Enum.GetValues(typeof(CountryCodeFormat));

            // Act
            var distinctCount = values.Length;

            // Assert - Make sure all values are unique
            Assert.That(distinctCount, Is.EqualTo(10)); // Total expected enum values
        }

        [Test]
        public void CountryCodeFormat_ShouldConvertToAndFromStrings()
        {
            // Test enum to string conversion
            Assert.That(CountryCodeFormat.Name.ToString(), Is.EqualTo("Name"));
            Assert.That(CountryCodeFormat.Alpha2.ToString(), Is.EqualTo("Alpha2"));
            Assert.That(CountryCodeFormat.Alpha3.ToString(), Is.EqualTo("Alpha3"));
            Assert.That(CountryCodeFormat.Numeric.ToString(), Is.EqualTo("Numeric"));

            // Test string to enum conversion
            Assert.That(Enum.Parse<CountryCodeFormat>("Name"), Is.EqualTo(CountryCodeFormat.Name));
            Assert.That(Enum.Parse<CountryCodeFormat>("OfficialName"), Is.EqualTo(CountryCodeFormat.OfficialName));
            Assert.That(Enum.Parse<CountryCodeFormat>("CallingCodeWithPlus"), Is.EqualTo(CountryCodeFormat.CallingCodeWithPlus));
            Assert.That(Enum.Parse<CountryCodeFormat>("Region"), Is.EqualTo(CountryCodeFormat.Region));
        }

        [Test]
        public void GetCountryFormat_WithAllFormats_ShouldReturnCorrectValue()
        {
            // Arrange
            var country = new Country
            {
                Name = "Test Country",
                OfficialName = "Republic of Test",
                Alpha2Code = "TC",
                Alpha3Code = "TCO",
                NumericCode = "999",
                CallingCode = "888",
                Region = "Test Region",
                Subregion = "Test Subregion",
                LocalizedNames = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "es", "País de Prueba" }
                }
            };

            // Act & Assert - Each format should return the corresponding property
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.Name), Is.EqualTo("Test Country"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.OfficialName), Is.EqualTo("Republic of Test"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.Alpha2), Is.EqualTo("TC"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.Alpha3), Is.EqualTo("TCO"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.Numeric), Is.EqualTo("999"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.CallingCode), Is.EqualTo("888"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.CallingCodeWithPlus), Is.EqualTo("+888"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.Region), Is.EqualTo("Test Region"));
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.Subregion), Is.EqualTo("Test Subregion"));

            // Localized name with culture code
            Assert.That(CountryMapper.GetCountryFormat(country, CountryCodeFormat.LocalizedName, "es"), Is.EqualTo("País de Prueba"));
        }
    }
}