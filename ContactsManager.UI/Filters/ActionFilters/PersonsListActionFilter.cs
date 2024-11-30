using ContactsManager.Core.Dto;
using ContactsManager.Core.Enums;
using ContactsManager.Ui.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Ui.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;
            IDictionary<string, object?>? actionArguments = (IDictionary<string, object?>?)context.HttpContext.Items["actionArguments"];
            if (actionArguments == null)
            {
                return;
            }

            // Data can be loaded to ViewData and ViewBag from OnActionExecuted() method using below
            if (actionArguments.ContainsKey("searchBy"))
            {
                personsController.ViewData["CurrentSearchBy"] = actionArguments["searchBy"];
            }
            if (actionArguments.ContainsKey("searchString"))
            {
                personsController.ViewData["CurrentSearchString"] = actionArguments["searchString"];
            }
            if (actionArguments.ContainsKey("sortBy"))
            {
                personsController.ViewData["CurrentSortBy"] = actionArguments["sortBy"];
            }
            else
            {
                personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
            }
            if (actionArguments.ContainsKey("sortOrder"))
            {
                personsController.ViewData["CurrentSortOrder"] = actionArguments["sortOrder"];
            }
            else
            {
                personsController.ViewData["CurrentSortOrder"] = SortOrderOptions.ASC;
            }

            personsController.ViewBag.SearchField = new Dictionary<string, string>
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Country), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

            context.HttpContext.Items["actionArguments"] = context.ActionArguments; // Add ActionArguments to context so it is accessible in OnActionExecuted() method

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (string.IsNullOrEmpty(searchBy))
                {
                    return;
                }

                List<string> searchByValidOptions =
                [
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.Country),
                    nameof(PersonResponse.Address),
                ];

                if (!searchByValidOptions.Any(validOption => searchBy == validOption))
                {
                    _logger.LogInformation("searchBy actual value: {searchBy}", searchBy);
                    context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName); // Replace argument value for PersonsController.Index() if needed
                    searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                    if (string.IsNullOrEmpty(searchBy))
                    {
                        return;
                    }
                    _logger.LogInformation("searchBy updated value: {searchBy}", searchBy);
                }
            }
        }
    }
}
