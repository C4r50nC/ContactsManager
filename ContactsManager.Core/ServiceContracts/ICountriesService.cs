using ContactsManager.Core.DTO;

namespace ContactsManager.Core.ServiceContracts
{
    /// <summary>
    /// Business logic representation for Country entity manipulation
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a Country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Added Country object with its new Guid</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list of countries
        /// </summary>
        /// <returns>All countries from the list as a list of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a CountryResponse object based on the given country ID
        /// </summary>
        /// <param name="countryId">Country ID for searching</param>
        /// <returns>Matching Country object as CountryResponse object</returns>
        Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);

        /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="formFile">Excel file with a list of countries</param>
        /// <returns>Number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
