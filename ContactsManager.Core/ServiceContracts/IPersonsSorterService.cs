using ContactsManager.Core.Dto;
using ContactsManager.Core.Enums;

namespace ContactsManager.Core.ServiceContracts
{
    /// <summary>
    /// Business logic representation for Person entity manipulation
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Returns a sorted list of all persons
        /// </summary>
        /// <param name="allPersons">List of all persons</param>
        /// <param name="sortBy">Name of the person's property which will be based on for sorting</param>
        /// <param name="sortOrder">Sorting order, which is either ASC or DESC</param>
        /// <returns>Sorted list of all persons as a list of PersonResponse</returns>
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
    }
}
