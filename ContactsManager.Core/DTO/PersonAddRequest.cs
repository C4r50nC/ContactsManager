using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO
{
    /// <summary>
    /// DTO class for adding a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required]
        public string? PersonName { get; set; }
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)] // Sets default type for all related views
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public GenderOptions? Gender { get; set; }
        [Required]
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }

        public Person ToPerson()
        {
            return new()
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryId = CountryId,
                Address = Address,
                ReceiveNewsletters = ReceiveNewsletters,
            };
        }
    }
}
