namespace CountryCodeKit
{
    /// <summary>
    /// Defines the available country code formats
    /// </summary>
    public enum CountryCodeFormat
    {
        /// <summary>
        /// The English name of the country (e.g., "United States")
        /// </summary>
        Name,

        /// <summary>
        /// The official name of the country (e.g., "United States of America")
        /// </summary>
        OfficialName,

        /// <summary>
        /// The ISO 3166-1 alpha-2 code (e.g., "US")
        /// </summary>
        Alpha2,

        /// <summary>
        /// The ISO 3166-1 alpha-3 code (e.g., "USA")
        /// </summary>
        Alpha3,

        /// <summary>
        /// The ISO 3166-1 numeric code (e.g., "840")
        /// </summary>
        Numeric,

        /// <summary>
        /// The calling code without the + prefix (e.g., "1")
        /// </summary>
        CallingCode,

        /// <summary>
        /// The calling code with the + prefix (e.g., "+1")
        /// </summary>
        CallingCodeWithPlus,

        /// <summary>
        /// The localized name of the country
        /// </summary>
        LocalizedName,

        /// <summary>
        /// The region (continent) of the country
        /// </summary>
        Region,

        /// <summary>
        /// The subregion of the country
        /// </summary>
        Subregion
    }
}