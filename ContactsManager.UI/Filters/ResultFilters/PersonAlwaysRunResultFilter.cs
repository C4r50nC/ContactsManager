using AspNetCrud.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.UI.Filters.ResultFilters
{
    public class PersonAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context) { }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Filters.OfType<SkipFilterAttribute>().Any())
            {
                return;
            }
        }
    }
}
