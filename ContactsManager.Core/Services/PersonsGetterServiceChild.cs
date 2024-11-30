using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.Dto;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Serilog;

namespace ContactsManager.Core.Services
{
    public class PersonsGetterServiceChild : PersonsGetterService
    {
        public PersonsGetterServiceChild(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
            : base(personsRepository, logger, diagnosticContext) { }

        public override async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new();
            using (ExcelPackage excelPackage = new(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Persons Sheet");
                // Write header row
                excelWorksheet.Cells["A1"].Value = "Person Name";
                excelWorksheet.Cells["B1"].Value = "Age";
                excelWorksheet.Cells["C1"].Value = "Gender";

                // Change header row style
                using (ExcelRange headerCells = excelWorksheet.Cells["A1:C1"])
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
                    excelWorksheet.Cells[rowNumber, 2].Value = personResponse.Age;
                    excelWorksheet.Cells[rowNumber, 3].Value = personResponse.Gender;

                    rowNumber++;
                }

                excelWorksheet.Cells[$"A1:C{rowNumber}"].AutoFitColumns(); // Auto adjust column width

                await excelPackage.SaveAsync(); // Save all written content
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
