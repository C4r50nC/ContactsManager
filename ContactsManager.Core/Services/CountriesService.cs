using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace ContactsManager.Core.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            ArgumentNullException.ThrowIfNull(countryAddRequest);

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("Country name already exists");
            }

            Country country = countryAddRequest.ToCountry();
            country.CountryId = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null)
            {
                return null;
            }

            Country? matchedCountry = await _countriesRepository.GetCountryByCountryId(countryId.Value);

            if (matchedCountry == null)
            {
                return null;
            }

            return matchedCountry.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new();
            await formFile.CopyToAsync(memoryStream); // Reads data from file to memory stream

            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = excelWorksheet.Dimension.Rows;
                for (int i = 2; i <= rowCount; i++) // Starting from 2 to skip the header
                {
                    string? cellValue = Convert.ToString(excelWorksheet.Cells[i, 1].Value);

                    if (string.IsNullOrEmpty(cellValue))
                    {
                        continue;
                    }

                    string countryName = cellValue;

                    if (await _countriesRepository.GetCountryByCountryName(countryName) != null)
                    {
                        continue;
                    }

                    Country country = new() { CountryName = cellValue };
                    await _countriesRepository.AddCountry(country);

                    countriesInserted++;
                }
            }

            return countriesInserted;
        }
    }
}
