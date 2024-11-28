using AspNetCrud.Filters;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Filters.ActionFilters;
using ContactsManager.UI.Filters.AuthorizationFilters;
using ContactsManager.UI.Filters.ExceptionFilters;
using ContactsManager.UI.Filters.ResourceFilters;
using ContactsManager.UI.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]")]
    // [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = ["X-Key-From-Controller", "Value-From-Controller", 3], Order = 3)]
    [ResponseHeaderFilterFactory("X-Key-From-Controller", "Value-From-Controller", 3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController
        (
            IPersonsGetterService personsGetterService
            , IPersonsAdderService personsAdderService
            , IPersonsSorterService personsSorterService
            , IPersonsDeleterService personsDeleterService
            , IPersonsUpdaterService personsUpdaterService
            , ICountriesService countriesService
            , ILogger<PersonsController> logger
        )
        {
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsSorterService = personsSorterService;
            _personsDeleterService = personsDeleterService;
            _personsUpdaterService = personsUpdaterService;
            _countriesService = countriesService;
            _logger = logger;
        }

        [Route("/")]
        [Route("[action]")]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]
        // [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = ["X-Custom-Key-From-Action", "Custom-Value-From-Action", 1], Order = 1)] // Custom key begins with "X-" is a convention
        [ResponseHeaderFilterFactory("X-Custom-Key-From-Action", "Custom-Value-From-Action", 1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

            List<PersonResponse> personResponses = await _personsGetterService.GetFilteredPersons(searchBy, searchString);
            List<PersonResponse> sortedPersonResponses = _personsSorterService.GetSortedPersons(personResponses, sortBy, sortOrder);
            return View(sortedPersonResponses);
        }

        [HttpGet]
        [Route("[action]")]
        // [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = ["X-Custom-Key-From-Action-2", "Custom-Value-From-Action-2", 4])]
        [ResponseHeaderFilterFactory("X-Custom-Key-From-Action-2", "Custom-Value-From-Action-2", 4)]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
            ViewBag.Countries = countryResponses.Select(country => new SelectListItem
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString(),
            });

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter))] // Disable person creation for resource filter demonstration
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            PersonResponse personResponse = await _personsAdderService.AddPerson(personRequest); // Variable personResponse can be used for further operations if needed

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid? personId)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
            ViewBag.Countries = countryResponses.Select(country => new SelectListItem
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString(),
            });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personRequest.PersonId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            PersonResponse updatedPersonResponse = await _personsUpdaterService.UpdatePerson(personRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid? personId)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personUpdateRequest.PersonId);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            await _personsDeleterService.DeletePerson(personResponse.PersonId);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPdf()
        {
            List<PersonResponse> persons = await _personsGetterService.GetAllPersons();

            return new ViewAsPdf("PersonsPdf", persons, ViewData)
            {
                PageMargins = new()
                {
                    Top = 20,
                    Right = 20,
                    Left = 20,
                    Bottom = 20,
                },

                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCsv()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsCsv();

            return File(memoryStream, "text/csv", "person.csv");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "person.xlsx");
        }
    }
}
