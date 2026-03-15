using MediatR;

namespace QuizNova.Application.Common.Behaviours;

using FluentValidation;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Common.Results.Abstractions;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // Mabye Some Requests Don't Have Validation Class
        if (validator is null)
        {
            return await next(ct);
        }

        var validationResult = await validator.ValidateAsync(request, ct);

        if (validationResult.IsValid)
        {
            return await next(ct);
        }

        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));

        return (dynamic)errors;
    }
}