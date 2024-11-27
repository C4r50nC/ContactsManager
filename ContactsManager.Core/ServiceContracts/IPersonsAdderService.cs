using ContactsManager.Core.DTO;

namespace ContactsManager.Core.ServiceContracts
{
    /// <summary>
    /// Business logic representation for Person entity manipulation
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds a Person object to the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person object to add</param>
        /// <returns>Added Person object with its new Guid</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}
