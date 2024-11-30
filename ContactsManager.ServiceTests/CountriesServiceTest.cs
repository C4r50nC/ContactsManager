using AutoFixture;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.Dto;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using Moq;

namespace ContactsManager.ServiceTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _countriesRepositoryMock = new();

            _countriesService = new CountriesService(_countriesRepositoryMock.Object);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            CountryAddRequest? countryAddRequest = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            CountryAddRequest? countryAddRequest = _fixture
                .Build<CountryAddRequest>()
                .With(country => country.CountryName, null as string)
                .Create();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public async Task AddCountry_DuplicatedCountryName_ToBeArgumentException()
        {
            CountryAddRequest countryAddRequest = _fixture
                .Build<CountryAddRequest>()
                .Create();

            Country country = countryAddRequest.ToCountry();

            _countriesRepositoryMock
                .Setup(countriesRepository => countriesRepository.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(country);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(countryAddRequest);
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
        {
            CountryAddRequest? countryAddRequest = _fixture.Create<CountryAddRequest>();

            Country country = countryAddRequest.ToCountry();

            _countriesRepositoryMock
                .Setup(countriesRepository => countriesRepository.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            Assert.True(countryResponse.CountryId != Guid.Empty);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeEmptyList()
        {
            _countriesRepositoryMock
                .Setup(countriesRepository => countriesRepository.GetAllCountries())
                .ReturnsAsync(new List<Country>());

            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();

            Assert.Empty(countryResponses);
        }

        [Fact]
        public async Task GetAllCountries_WithFewCountries_ToBeSuccessful()
        {
            List<Country> countries = [
                _fixture.Build<Country>().With(country => country.Persons, null as List<Person>).Create(),
                _fixture.Build<Country>().With(country => country.Persons, null as List<Person>).Create(),
            ];

            List<CountryResponse> expectedCountriesList = countries.Select(country => country.ToCountryResponse()).ToList();

            _countriesRepositoryMock
                .Setup(countriesRepository => countriesRepository.GetAllCountries())
                .ReturnsAsync(countries);

            List<CountryResponse> actualCountriesList = await _countriesService.GetAllCountries();

            foreach (CountryResponse expectedCountry in expectedCountriesList)
            {
                Assert.Contains(expectedCountry, actualCountriesList);
            }
        }

        #endregion

        #region GetCountryByCountryId

        [Fact]
        public async Task GetCountryByCountryId_NullCountryId_ToBeNull()
        {
            Guid? countryId = null;

            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(countryId);

            Assert.Null(countryResponse);
        }

        [Fact]
        public async Task GetCountryByCountryId_ValidCountryId_ToBeSuccessful()
        {
            Country country = _fixture
                .Build<Country>()
                .With(country => country.Persons, null as List<Person>)
                .Create();

            CountryResponse countryResponse = country.ToCountryResponse();

            _countriesRepositoryMock
                .Setup(countriesRepository => countriesRepository.GetCountryByCountryId(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            CountryResponse? actualCountryResponse = await _countriesService.GetCountryByCountryId(countryResponse.CountryId);

            Assert.Equal(countryResponse, actualCountryResponse);
        }

        #endregion
    }
}
