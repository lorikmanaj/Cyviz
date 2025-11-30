using Cyviz.Core.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cyviz.Filters
{
    public class ApiRequireOperatorAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var caller = context.HttpContext.Items["CallerType"] as ApiCallerType?;

            if (caller != ApiCallerType.Operator)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Operator API key required"
                });
                return;
            }

            await next();
        }
    }
}
