using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.ServiceContracts;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Serilog;
using SerilogTimings;
using System.Globalization;

namespace ContactsManager.Core.Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");

            List<Person> persons = await _personsRepository.GetAllPersons();

            return persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public virtual async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
        {
            if (personId == null)
            {
                return null;
            }

            Person? personResult = await _personsRepository.GetPersonByPersonId(personId.Value);
            if (personResult == null)
            {
                return null;
            }

            return personResult.ToPersonResponse();
        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");

            if (searchString == null)
            {
                return await GetAllPersons();
            }

            List<Person> matchingPersons;

            using (Operation.Time("Time for GetFilteredPersons() from database"))
            {
                matchingPersons = searchBy switch
                {
                    // PersonsService.GetAllPersons() should be avoided for large datasets to reduce memory consumption. Operations should be done in the database directly

                    // Expressions in GetFilteredPersons() will be converted to equivalent valid SQL queries at runtime
                    nameof(PersonResponse.PersonName) => await _personsRepository.GetFilteredPersons(
                        person =>
                            string.IsNullOrEmpty(person.PersonName) || person.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) => await _personsRepository.GetFilteredPersons(
                        person =>
                            string.IsNullOrEmpty(person.Email) || person.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) => await _personsRepository.GetFilteredPersons(
                        person =>
                            !person.DateOfBirth.HasValue || person.DateOfBirth.Value.ToString().Contains(searchString)),

                    nameof(PersonResponse.Gender) => await _personsRepository.GetFilteredPersons(
                        person =>
                            string.IsNullOrEmpty(person.Gender) || person.Gender.Equals(searchString)),

                    nameof(PersonResponse.Country) => await _personsRepository.GetFilteredPersons(
                        person =>
                            !person.CountryId.HasValue
                            || person.Country == null
                            || person.Country.CountryName == null
                            || person.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) => await _personsRepository.GetFilteredPersons(
                        person =>
                            string.IsNullOrEmpty(person.Address) || person.Address.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons(),
                };
            }

            _diagnosticContext.Set("Persons", matchingPersons);

            return matchingPersons.Select(person => person.ToPersonResponse()).ToList();
        }

        public virtual async Task<MemoryStream> GetPersonsCsv()
        {
            MemoryStream memoryStream = new();
            StreamWriter streamWriter = new(memoryStream);

            CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new(streamWriter, csvConfiguration);

            // Manually write required headers
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsletters));
            await csvWriter.NextRecordAsync(); // New line in CSV

            List<PersonResponse> personResponses = await GetAllPersons();
            foreach (PersonResponse personResponse in personResponses)
            {
                csvWriter.WriteField(personResponse.PersonName);
                csvWriter.WriteField(personResponse.Email);
                csvWriter.WriteDateOfBirth(personResponse.DateOfBirth);
                csvWriter.WriteField(personResponse.Age);
                csvWriter.WriteField(personResponse.Gender);
                csvWriter.WriteField(personResponse.Country);
                csvWriter.WriteField(personResponse.Address);
                csvWriter.WriteField(personResponse.ReceiveNewsletters);

                await csvWriter.NextRecordAsync(); // Add new line after each person
            }

            await csvWriter.FlushAsync(); // Write data in CSV writer to memory stream

            memoryStream.Position = 0; // Reset the cursor after writing all data
            return memoryStream;
        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new();
            using (ExcelPackage excelPackage = new(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Persons Sheet");
                // Write header row
                excelWorksheet.Cells["A1"].Value = "Person Name";
                excelWorksheet.Cells["B1"].Value = "Email";
                excelWorksheet.Cells["C1"].Value = "Date of Birth";
                excelWorksheet.Cells["D1"].Value = "Age";
                excelWorksheet.Cells["E1"].Value = "Gender";
                excelWorksheet.Cells["F1"].Value = "Country";
                excelWorksheet.Cells["G1"].Value = "Address";
                excelWorksheet.Cells["H1"].Value = "Receive Newsletters";

                // Change header row style
                using (ExcelRange headerCells = excelWorksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int rowNumber = 2;
                List<PersonResponse> personResponses = await GetAllPersons();
                foreach (PersonResponse personResponse in personResponses)
                {
                    excelWorksheet.Cells[rowNumber, 1].Value = personResponse.PersonName;
                    excelWorksheet.Cells[rowNumber, 2].Value = personResponse.Email;
                    if (personResponse.DateOfBirth.HasValue)
                    {
                        excelWorksheet.Cells[rowNumber, 3].Value = personResponse.DateOfBirth.Value.ToString("yyyy-MMM-dd");
                    }
                    else
                    {
                        excelWorksheet.Cells[rowNumber, 3].Value = "";
                    }
                    excelWorksheet.Cells[rowNumber, 4].Value = personResponse.Age;
                    excelWorksheet.Cells[rowNumber, 5].Value = personResponse.Gender;
                    excelWorksheet.Cells[rowNumber, 6].Value = personResponse.Country;
                    excelWorksheet.Cells[rowNumber, 7].Value = personResponse.Address;
                    excelWorksheet.Cells[rowNumber, 8].Value = personResponse.ReceiveNewsletters;

                    rowNumber++;
                }

                excelWorksheet.Cells[$"A1:H{rowNumber}"].AutoFitColumns(); // Auto adjust column width

                await excelPackage.SaveAsync(); // Save all written content
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }

    public static class CsvWriterExtensions
    {
        public static void WriteDateOfBirth(this CsvWriter csvWriter, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                csvWriter.WriteField(dateTime.Value.ToString("yyyy-MMM-dd"));
                return;
            }

            csvWriter.WriteField("");
        }
    }
}
