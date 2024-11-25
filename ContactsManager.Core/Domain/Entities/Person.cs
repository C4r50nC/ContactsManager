using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsManager.Core.Domain.Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }
        [StringLength(40)]
        public string? PersonName { get; set; }
        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }
        // Need to update stored procedures in PersonsDatabase to function properly
        public string? TaxNumber { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"Person ID: {PersonId}, " +
                $"Person Name: {PersonName}, " +
                $"Email: {Email}, " +
                $"Date of Birth: {DateOfBirth?.ToString("MMM/dd/yyyy")}, " +
                $"Gender: {Gender}, " +
                $"Country ID: {CountryId}, " +
                $"Country: {Country?.CountryName}, " +
                $"Address: {Address}, " +
                $"Receive Newsletters {ReceiveNewsletters}";
        }
    }
}
