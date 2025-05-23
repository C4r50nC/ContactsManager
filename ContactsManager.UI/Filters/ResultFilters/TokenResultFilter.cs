﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Ui.Filters.ResultFilters
{
    public class TokenResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context) { }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Append("Auth-Key", "A100"); // Must match with TokenAuthorizationFilter for demonstration
        }
    }
}
