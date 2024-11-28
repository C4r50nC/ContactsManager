using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace ContactsManager.Infrastructure.Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PersonsRepository> _logger;

        public PersonsRepository(ApplicationDbContext dbContext, ILogger<PersonsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _dbContext.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _dbContext.RemoveRange(_dbContext.Persons.Where(person => person.PersonId == personId));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _dbContext.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsRepository");

            return await _dbContext.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _dbContext.Persons.Include("Country").FirstOrDefaultAsync(person => person.PersonId == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _dbContext.Persons.FirstOrDefaultAsync(p => p.PersonId == person.PersonId);

            if (matchingPerson == null)
            {
                return person;
            }

            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryId = person.CountryId;
            matchingPerson.Address = person.Address;
            matchingPerson.ReceiveNewsletters = person.ReceiveNewsletters;

            int rowsUpdated = await _dbContext.SaveChangesAsync();
            return matchingPerson;
        }
    }
}
