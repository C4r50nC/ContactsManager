using AutoFixture;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace ContactsManager.ServiceTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;

        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _personsRepositoryMock = new();

            Mock<ILogger<PersonsGetterService>> getterLoggerMock = new();
            Mock<ILogger<PersonsAdderService>> adderLoggerMock = new();
            Mock<ILogger<PersonsDeleterService>> deleterLoggerMock = new();
            Mock<ILogger<PersonsSorterService>> sorterLoggerMock = new();
            Mock<ILogger<PersonsUpdaterService>> updaterLoggerMock = new();
            Mock<IDiagnosticContext> diagnosticContextMock = new();

            _personsGetterService = new PersonsGetterService(_personsRepositoryMock.Object, getterLoggerMock.Object, diagnosticContextMock.Object);
            _personsAdderService = new PersonsAdderService(_personsRepositoryMock.Object, adderLoggerMock.Object, diagnosticContextMock.Object);
            _personsDeleterService = new PersonsDeleterService(_personsRepositoryMock.Object, deleterLoggerMock.Object, diagnosticContextMock.Object);
            _personsSorterService = new PersonsSorterService(_personsRepositoryMock.Object, sorterLoggerMock.Object, diagnosticContextMock.Object);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepositoryMock.Object, updaterLoggerMock.Object, diagnosticContextMock.Object);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            PersonAddRequest? personAddRequest = null;

            Func<Task> addPersonAction = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };

            await addPersonAction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            PersonAddRequest personAddRequest = _fixture
                .Build<PersonAddRequest>()
                .With(person => person.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();
            // All repository methods invoked in _personsService need to be mocked
            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.AddPerson(It.IsAny<Person>())) // It.IsAny() takes any object of type <T>
                .ReturnsAsync(person); // Fixed return value

            Func<Task> addPersonAction = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };

            await addPersonAction.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails_ToBeSuccessful()
        {
            PersonAddRequest personAddRequest = _fixture
                .Build<PersonAddRequest>()
                .With(person => person.Email, "someone@example.com")
                .Create(); // AutoFixture automatically generates all properties, and custom properties are specified in _fixture.Build().With()

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse personResponse = await _personsAdderService.AddPerson(personAddRequest);
            personResponseExpected.PersonId = personResponse.PersonId;

            personResponse.PersonId.Should().NotBe(Guid.Empty);
            personResponse.Should().Be(personResponseExpected);
        }

        #endregion

        #region GetPersonByPersonId

        [Fact]
        public async Task GetPersonByPersonId_NullPersonId_ToBeNull()
        {
            Guid? personId = null;

            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personId);

            personResponse.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonId_ValidPersonId_ToBeSuccessful()
        {
            Person person = _fixture
                .Build<Person>()
                .With(person => person.Email, "email@sample.com")
                .With(person => person.Country, null as Country)
                .Create();
            PersonResponse expectedPersonResponse = person.ToPersonResponse();

            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            PersonResponse? actualPersonResponse = await _personsGetterService.GetPersonByPersonId(person.PersonId);

            actualPersonResponse.Should().BeEquivalentTo(expectedPersonResponse);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_InitialState_ToBeEmptyList()
        {
            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.GetAllPersons())
                .ReturnsAsync(new List<Person>());

            List<PersonResponse> personResponses = await _personsGetterService.GetAllPersons();

            personResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            List<Person> persons =
                [
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Mary")
                        .With(person => person.Email, "email1@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Rahman")
                        .With(person => person.Email, "email2@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Scott")
                        .With(person => person.Email, "email3@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                ];

            List<PersonResponse> expectedPersonResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> actualPersonResponses = await _personsGetterService.GetAllPersons();
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in actualPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            actualPersonResponses.Should().BeEquivalentTo(expectedPersonResponses);
        }

        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchString_ToBeSuccessful()
        {
            List<Person> persons =
                [
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Mary")
                        .With(person => person.Email, "email1@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Rahman")
                        .With(person => person.Email, "email2@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Scott")
                        .With(person => person.Email, "email3@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                ];

            List<PersonResponse> expectedPersonResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _personsRepositoryMock
                .Setup(personsRepository => personsRepository
                .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            List<PersonResponse> actualPersonResponses = await _personsGetterService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in actualPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            actualPersonResponses.Should().BeEquivalentTo(expectedPersonResponses);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful() // This test is inactive because both actual and expected results are hard-coded
        {
            List<Person> persons =
                [
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Mary")
                        .With(person => person.Email, "email1@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Rahman")
                        .With(person => person.Email, "email2@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Scott")
                        .With(person => person.Email, "email3@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                ];

            List<PersonResponse> expectedPersonResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in expectedPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _personsRepositoryMock.Setup(personsRepository => personsRepository
                .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            List<PersonResponse> actualPersonResponses = await _personsGetterService.GetFilteredPersons(nameof(PersonResponse.PersonName), "ma");
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in actualPersonResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            actualPersonResponses.Should().BeEquivalentTo(expectedPersonResponses);
        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_SortByPersonNameDesc_ToBeSuccessful()
        {
            List<Person> persons =
                [
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Mary")
                        .With(person => person.Email, "email1@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Rahman")
                        .With(person => person.Email, "email2@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                    _fixture
                        .Build<Person>()
                        .With(person => person.PersonName, "Scott")
                        .With(person => person.Email, "email3@email.com")
                        .With(person => person.Country, null as Country)
                        .Create(),
                ];

            List<PersonResponse> expectedPersonResponses = persons.Select(person => person.ToPersonResponse()).ToList();

            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> allPersons = await _personsGetterService.GetAllPersons();
            List<PersonResponse> actualPersonResponses = _personsSorterService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);

            actualPersonResponses.Should().BeInDescendingOrder(person => person.PersonName);
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            Func<Task> updatePersonAction = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            await updatePersonAction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
        {
            // Guid will be generated by AutoFixture, so it will not be an existing ID
            PersonUpdateRequest personUpdateRequest = _fixture
                .Build<PersonUpdateRequest>()
                .With(person => person.Email, "email@email.com")
                .Create();

            Func<Task> updatePersonAction = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            await updatePersonAction.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            Person person = _fixture
                .Build<Person>()
                .With(person => person.PersonName, null as string)
                .With(person => person.Email, "email@email.com")
                .With(person => person.Gender, "Other") // Gender must be specified because _fixture supplies a random string which causes error when converting to enum in ToPersonResponse()
                .With(person => person.Country, null as Country)
                .Create();

            PersonResponse personResponse = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            Func<Task> updatePersonAction = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            await updatePersonAction.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            Person person = _fixture
                .Build<Person>()
                .With(person => person.Email, "email@email.com")
                .With(person => person.Gender, "Other")
                .With(person => person.Country, null as Country)
                .Create();

            PersonResponse expectedPersonResponse = person.ToPersonResponse();

            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            PersonUpdateRequest personUpdateRequest = expectedPersonResponse.ToPersonUpdateRequest();
            PersonResponse actualPersonResponse = await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            actualPersonResponse.Should().Be(expectedPersonResponse);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ValidPersonId_ToBeSuccessful()
        {
            Person person = _fixture
                .Build<Person>()
                .With(person => person.Email, "email@email.com")
                .With(person => person.Country, null as Country)
                .Create();

            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.DeletePersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _personsRepositoryMock
                .Setup(personsRepository => personsRepository.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            bool personIsDeleted = await _personsDeleterService.DeletePerson(person.PersonId);

            personIsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePerson_InvalidPersonId_ToBeFalse()
        {
            bool personIsDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            personIsDeleted.Should().BeFalse();
        }

        #endregion
    }
}
