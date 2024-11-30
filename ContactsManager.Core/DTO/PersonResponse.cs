using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Enums;

namespace ContactsManager.Core.Dto
{
    /// <summary>
    /// DTO class that is used as the return type for PersonsService methods
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Age { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }

            PersonResponse personResponse = (PersonResponse)obj;
            return true
                && personResponse.PersonId == PersonId
                && personResponse.PersonName == PersonName
                && personResponse.Email == Email
                && personResponse.DateOfBirth == DateOfBirth
                && personResponse.Age == Age
                && personResponse.Gender == Gender
                && personResponse.CountryId == CountryId
                && personResponse.Country == Country
                && personResponse.Address == Address
                && personResponse.ReceiveNewsletters == ReceiveNewsletters
            ;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonId}, " +
            $"Person Name: {PersonName}, " +
            $"Email: {Email}, " +
            $"Date of Birth: {DateOfBirth?.ToString("MM dd yyyy")}, " +
            $"Age: {Age}, " +
            $"Gender: {Gender}, " +
            $"Country ID: {CountryId}, " +
            $"Country: {Country}, " +
            $"Address: {Address}, " +
            $"Receive Newsletters: {ReceiveNewsletters.ToString()}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest
            {
                PersonId = PersonId,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender == null ? null : (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                CountryId = CountryId,
                Address = Address,
                ReceiveNewsletters = ReceiveNewsletters,
            };
        }
    }

    public static class PersonExtensions
    {
        // Injects to Person class as an available method. Implemented in DTO class so the entity class can be kept clean
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Age = person.DateOfBirth != null ? Math.Round((DateTime.Now - person.DateOfBirth).Value.TotalDays / 365.25) : null,
                Gender = person.Gender,
                CountryId = person.CountryId,
                Address = person.Address,
                ReceiveNewsletters = person.ReceiveNewsletters,
                Country = person.Country?.CountryName,
            };
        }
    }
}
