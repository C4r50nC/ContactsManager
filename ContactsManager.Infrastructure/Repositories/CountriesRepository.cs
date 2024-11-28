using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CountriesRepository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public async Task<Country> AddCountry(Country country)
        {
            await _dbContext.AddAsync(country);
            await _dbContext.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _dbContext.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryId(Guid countryId)
        {
            return await _dbContext.Countries.FirstOrDefaultAsync(country => country.CountryId == countryId);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _dbContext.Countries.FirstOrDefaultAsync(country => country.CountryName == countryName);
        }
    }
}
