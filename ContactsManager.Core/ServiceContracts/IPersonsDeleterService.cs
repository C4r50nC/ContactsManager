namespace ServiceContracts
{
    /// <summary>
    /// Business logic representation for Person entity manipulation
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Delete the person specified by the person ID
        /// </summary>
        /// <param name="personId">Person ID of the person to delete</param>
        /// <returns>True if deletion is successful and false if deletion is unsuccessful</returns>
        Task<bool> DeletePerson(Guid? personId);
    }
}
