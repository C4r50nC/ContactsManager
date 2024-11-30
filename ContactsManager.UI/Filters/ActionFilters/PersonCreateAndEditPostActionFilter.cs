using ContactsManager.Core.Dto;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Ui.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContactsManager.Ui.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;

        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is not PersonsController personsController)
            {
                await next();
                return;
            }

            if (!personsController.ModelState.IsValid)
            {
                List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
                personsController.ViewBag.Countries = countryResponses.Select(country => new SelectListItem
                {
                    Text = country.CountryName,
                    Value = country.CountryId.ToString(),
                });

                personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();

                // Action method arguments in the controller need to be renamed so the filter can be applied to multiple action methods
                object? personRequest = context.ActionArguments["personRequest"];
                context.Result = personsController.View(personRequest); // Send view result to the browser directly to short circuit processes (filters and action methods) after this filter

                return;
            }

            await next();
        }
    }
}
