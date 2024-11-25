using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Data management logic for Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add a new country object to the data source
        /// </summary>
        /// <param name="country">Country object to be added</param>
        /// <returns>Country object added to the data source</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Get all countries from the data source
        /// </summary>
        /// <returns>List of all countries from the data source</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Find the country object with given country ID
        /// </summary>
        /// <param name="countryId">Country ID to search</param>
        /// <returns>Country object found or null if not found</returns>
        Task<Country?> GetCountryByCountryId(Guid countryId);

        /// <summary>
        /// Find the country object with given country name
        /// </summary>
        /// <param name="countryName">Country name to search</param>
        /// <returns>Country object found or null if not found</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
