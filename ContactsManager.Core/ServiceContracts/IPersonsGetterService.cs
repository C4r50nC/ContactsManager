using ContactsManager.Core.Dto;

namespace ContactsManager.Core.ServiceContracts
{
    /// <summary>
    /// Business logic representation for Person entity manipulation
    /// </summary>
    public interface IPersonsGetterService
    {
        /// <summary>
        /// Returns all persons from the list of persons
        /// </summary>
        /// <returns>All persons from the list as a list of PersonResponse</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Returns a PersonResponse object based on the given person ID
        /// </summary>
        /// <param name="personId">Person ID for searching</param>
        /// <returns>Matching Person object as PersonResponse object</returns>
        Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>All matching persons based on the given search field and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns data for all persons as CSV
        /// </summary>
        /// <returns>Memory stream with CSV data of all persons</returns>
        Task<MemoryStream> GetPersonsCsv();

        /// <summary>
        /// Returns data for all persons as Excel
        /// </summary>
        /// <returns>Memory stream with Excel data of all persons</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
