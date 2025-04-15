namespace CountryCodeKit
{
    /// <summary>
    /// Represents a comprehensive country information container
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the English name of the country
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the official name of the country
        /// </summary>
        public string OfficialName { get; set; }

        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 code (two-letter)
        /// </summary>
        public string Alpha2Code { get; set; }

        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-3 code (three-letter)
        /// </summary>
        public string Alpha3Code { get; set; }

        /// <summary>
        /// Gets or sets the ISO 3166-1 numeric code
        /// </summary>
        public string NumericCode { get; set; }

        /// <summary>
        /// Gets or sets the calling code (without + prefix)
        /// </summary>
        public string CallingCode { get; set; }

        /// <summary>
        /// Gets or sets the region (continent) of the country
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the subregion of the country
        /// </summary>
        public string Subregion { get; set; }

        /// <summary>
        /// Gets or sets alternative names/spellings for the country
        /// </summary>
        public List<string> AlternativeNames { get; set; } = [];

        /// <summary>
        /// Gets or sets localized names of the country (key is the culture code)
        /// </summary>
        public Dictionary<string, string> LocalizedNames { get; set; } = [];
    }
}