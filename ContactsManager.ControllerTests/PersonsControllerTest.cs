using AutoFixture;
using ContactsManager.Core.Dto;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Ui.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactsManager.ControllerTests
{
    public class PersonsControllerTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new();
            _personsGetterServiceMock = new();
            _personsAdderServiceMock = new();
            _personsDeleterServiceMock = new();
            _personsSorterServiceMock = new();
            _personsUpdaterServiceMock = new();
            _loggerMock = new();

            _countriesService = _countriesServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _logger = _loggerMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ToReturnIndexViewWithPersonsList()
        {
            List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new
            (
                _personsGetterService
                , _personsAdderService
                , _personsSorterService
                , _personsDeleterService
                , _personsUpdaterService
                , _countriesService
                , _logger
            );

            _personsGetterServiceMock
                .Setup(personsGetterService => personsGetterService.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(personResponses);
            _personsSorterServiceMock
                .Setup(personsSorterService => personsSorterService.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .Returns(personResponses);

            IActionResult indexResult = await personsController.Index
                (
                    _fixture.Create<string>(),
                    _fixture.Create<string>(),
                    _fixture.Create<string>(),
                    _fixture.Create<SortOrderOptions>()
                );

            ViewResult viewResult = Assert.IsType<ViewResult>(indexResult);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(personResponses);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_ModelIsValid_ToRedirectToIndex()
        {
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();

            PersonResponse personResponse = _fixture.Create<PersonResponse>();

            List<CountryResponse> countryResponses = _fixture.Create<List<CountryResponse>>();
            _countriesServiceMock
                .Setup(countriesService => countriesService.GetAllCountries())
                .ReturnsAsync(countryResponses);

            _personsAdderServiceMock
                .Setup(personsService => personsService.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new
            (
                _personsGetterService
                , _personsAdderService
                , _personsSorterService
                , _personsDeleterService
                , _personsUpdaterService
                , _countriesService
                , _logger
            );

            IActionResult createResult = await personsController.Create(personAddRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(createResult);
            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
