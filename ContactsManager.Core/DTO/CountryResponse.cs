using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as the return type for CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }

            CountryResponse countryResponse = (CountryResponse)obj;
            // CountryId and CountryName uses value from "this" keyword by default
            return (CountryId == countryResponse.CountryId) && (CountryName == countryResponse.CountryName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CountryExtensions
    {
        // Injects to Country class as an available method. Implemented in DTO class so the entity class can be kept clean
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }
}
