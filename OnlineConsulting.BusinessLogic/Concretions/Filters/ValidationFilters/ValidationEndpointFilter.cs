using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineConsulting.BusinessLogic.Concretions.Filters.ValidationFilters;

public class ValidationEndpointFilter<TDto>(IServiceProvider serviceProvider) : IEndpointFilter where TDto : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.FirstOrDefault(arg => arg?.GetType() == typeof(TDto));

        if (model is null)
            return Results.BadRequest("Model binding failed.");

        // Resolve validator
        var validator = serviceProvider.GetService<IValidator<TDto>>();
        if (validator is null)
            return Results.Problem("Validator not found for model type.");

        var validationResult = await validator.ValidateAsync((TDto)model);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return Results.BadRequest(errors);
        }

        return await next(context);
    }
}
