using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            ArgumentNullException.ThrowIfNull(personUpdateRequest);

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId);
            if (matchingPerson == null)
            {
                throw new InvalidPersonIdException("Given person ID does not exist");
            }

            await _personsRepository.UpdatePerson(matchingPerson);

            return matchingPerson.ToPersonResponse();
        }
    }
}
