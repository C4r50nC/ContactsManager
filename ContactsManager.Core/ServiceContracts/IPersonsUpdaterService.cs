using ContactsManager.Core.DTO;

namespace ContactsManager.Core.ServiceContracts
{
    /// <summary>
    /// Business logic representation for Person entity manipulation
    /// </summary>
    public interface IPersonsUpdaterService
    {

        /// <summary>
        /// Update person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update including the person ID</param>
        /// <returns>PersonResponse object of updated person details</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
