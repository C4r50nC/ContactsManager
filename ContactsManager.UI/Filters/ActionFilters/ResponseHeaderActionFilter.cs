using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Ui.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        private string ResponseHeaderKey { get; set; }
        private string ResponseHeaderValue { get; set; }
        private int Order { get; set; }

        public ResponseHeaderFilterFactoryAttribute(string responseHeaderKey, string responseHeaderValue, int order)
        {
            ResponseHeaderKey = responseHeaderKey;
            ResponseHeaderValue = responseHeaderValue;
            Order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            ResponseHeaderActionFilter responseHeaderActionFilter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            responseHeaderActionFilter.ResponseHeaderKey = ResponseHeaderKey;
            responseHeaderActionFilter.ResponseHeaderValue = ResponseHeaderValue;
            responseHeaderActionFilter.Order = Order;
            return responseHeaderActionFilter;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        public string? ResponseHeaderKey { get; set; }
        public string? ResponseHeaderValue { get; set; }
        public int Order { get; set; }

        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        // Constructor must only contain arguments for dependency injection
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // From OnActionExecuting
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            if (ResponseHeaderKey != null)
            {
                context.HttpContext.Response.Headers[ResponseHeaderKey] = ResponseHeaderValue;
            }

            // await next() must be included to invoke the next filter in order
            await next(); // If there are no filters left, the action method will be invoked

            // From OnActionExecuted
            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
        }
    }
}
