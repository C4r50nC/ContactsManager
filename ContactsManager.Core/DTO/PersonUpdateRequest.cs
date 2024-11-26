using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class for updating an existing person
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required]
        public Guid PersonId { get; set; }
        [Required]
        public string? PersonName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }

        public Person ToPerson()
        {
            return new()
            {
                PersonId = PersonId,
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
