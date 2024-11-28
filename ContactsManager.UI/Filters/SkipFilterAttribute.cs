using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCrud.Filters
{
    public class SkipFilterAttribute : Attribute, IFilterMetadata // IFilterMetadata is mandatory for all filters so filter information can be accessed elsewhere
    {
    }
}
