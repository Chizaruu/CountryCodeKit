using System.Collections.Generic;
using NUnit.Framework;

namespace CountryCodeKit.Tests
{
    [TestFixture]
    public class CountryEntityTests
    {
        [Test]
        public void Country_PropertiesInitializeCorrectly()
        {
            // Arrange
            var country = new Country
            {
                Name = "TestLand",
                OfficialName = "Republic of TestLand",
                Alpha2Code = "TL",
                Alpha3Code = "TLD",
                NumericCode = "123",
                CallingCode = "321",
                Region = "Test Region",
                Subregion = "Test Subregion",
                AlternativeNames = new List<string> { "Test Country", "TL Nation" },
                LocalizedNames = new Dictionary<string, string>
                {
                    { "es", "País de Prueba" },
                    { "fr", "Pays de Test" },
                    { "de", "Testland" }
                }
            };

            // Assert
            Assert.That(country.Name, Is.EqualTo("TestLand"));
            Assert.That(country.OfficialName, Is.EqualTo("Republic of TestLand"));
            Assert.That(country.Alpha2Code, Is.EqualTo("TL"));
            Assert.That(country.Alpha3Code, Is.EqualTo("TLD"));
            Assert.That(country.NumericCode, Is.EqualTo("123"));
            Assert.That(country.CallingCode, Is.EqualTo("321"));
            Assert.That(country.Region, Is.EqualTo("Test Region"));
            Assert.That(country.Subregion, Is.EqualTo("Test Subregion"));

            // Assert collections
            Assert.That(country.AlternativeNames.Count, Is.EqualTo(2));
            CollectionAssert.Contains(country.AlternativeNames, "Test Country");
            CollectionAssert.Contains(country.AlternativeNames, "TL Nation");

            Assert.That(country.LocalizedNames.Count, Is.EqualTo(3));
            Assert.That(country.LocalizedNames["es"], Is.EqualTo("País de Prueba"));
            Assert.That(country.LocalizedNames["fr"], Is.EqualTo("Pays de Test"));
            Assert.That(country.LocalizedNames["de"], Is.EqualTo("Testland"));
        }

        [Test]
        public void Country_CollectionsDefaultToEmptyNotNull()
        {
            // Arrange
            var country = new Country
            {
                Name = "Minimal Country",
                Alpha2Code = "MC"
            };

            // Assert
            Assert.That(country.AlternativeNames, Is.Not.Null);
            Assert.IsEmpty(country.AlternativeNames);

            Assert.That(country.LocalizedNames, Is.Not.Null);
            Assert.IsEmpty(country.LocalizedNames);
        }

        [Test]
        public void Country_CanModifyCollections()
        {
            // Arrange
            var country = new Country();

            // Act - Add to AlternativeNames
            country.AlternativeNames.Add("Alt1");
            country.AlternativeNames.Add("Alt2");

            // Act - Add to LocalizedNames
            country.LocalizedNames["es"] = "Spanish Name";
            country.LocalizedNames["fr"] = "French Name";

            // Assert
            Assert.That(country.AlternativeNames.Count, Is.EqualTo(2));
            CollectionAssert.Contains(country.AlternativeNames, "Alt1");
            CollectionAssert.Contains(country.AlternativeNames, "Alt2");

            Assert.That(country.LocalizedNames.Count, Is.EqualTo(2));
            Assert.That(country.LocalizedNames["es"], Is.EqualTo("Spanish Name"));
            Assert.That(country.LocalizedNames["fr"], Is.EqualTo("French Name"));
        }
    }
}