using ContactsManager.Core.Domain.Entities;
using System.Linq.Expressions;

namespace ContactsManager.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Data management logic for Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Add a new person object to the data source
        /// </summary>
        /// <param name="person">Person object to be added</param>
        /// <returns>Person object added to the data source</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Get all persons from the data source
        /// </summary>
        /// <returns>List of all persons from the data source</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Find the person object with given person ID
        /// </summary>
        /// <param name="personId">Person ID to search</param>
        /// <returns>Person object found or null if not found</returns>
        Task<Person?> GetPersonByPersonId(Guid personId);

        /// <summary>
        /// Find all person objects matching with the given expression
        /// </summary>
        /// <param name="predicate">Expression to check if the person matches</param>
        /// <returns>All person objects matching with the criteria</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Delete the person object with given ID from the data source
        /// </summary>
        /// <param name="personId">Person ID to search</param>
        /// <returns>True if deletion successful and false if unsuccessful</returns>
        Task<bool> DeletePersonByPersonId(Guid personId);

        /// <summary>
        /// Update a person object based on the given person object
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Updated person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
