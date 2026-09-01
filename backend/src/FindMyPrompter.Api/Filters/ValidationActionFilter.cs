using FindMyPrompter.Application.Messages;
using FindMyPrompter.Application.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FindMyPrompter.Api.Filters;

/// <summary>
/// Intercepta actions cujos argumentos implementam <see cref="IValidatableRequest"/>.
/// Chama <see cref="IValidatableRequest.Validate"/> em cada um e devolve HTTP 400
/// quando a validação falha, sem executar a action.
/// </summary>
public sealed class ValidationActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var value in context.ActionArguments.Values)
        {
            if (value is not IValidatableRequest validatable)
            {
                continue;
            }

            var result = validatable.Validate();

            if (result.IsValid)
            {
                continue;
            }

            var errors = result.Errors
                .Select(failure => new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["propertyName"] = failure.PropertyName,
                    ["errorMessage"] = failure.ErrorMessage
                })
                .ToList<object?>();

            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = Messages.Validation.OneOrMoreErrorsOccurred,
                Type = "about:blank",
                Extensions = { ["errors"] = errors }
            });

            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
