using FluentValidation.Results;

namespace realtor_notes_backend.utils;

using ErrorOr;

public static class ResultExtensions
{
    public static ErrorOr<T> ToErrorOr<T>(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
            .ToList();
    }
    
    public static IResult ToProblemDetails<T>(this ErrorOr<T> result)
    {
        if (!result.IsError)
        {
            throw new InvalidOperationException("Нельзя вызвать ToProblemDetails для успешного результата.");
        }

        // Если ошибка всего одна
        if (result.Errors.Count == 1)
        {
            return ToSingleErrorProblem(result.FirstError);
        }

        // Если ошибок несколько (например, валидация)
        return ToMultipleErrorsProblem(result.Errors);
    }

    private static IResult ToSingleErrorProblem(Error error)
    {
        var statusCode = GetStatusCode(error.Type);

        return Results.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description
        );
    }

    private static IResult ToMultipleErrorsProblem(List<Error> errors)
    {
        // Кастомный словарь для описания множественных ошибок (например, для фронтенда)
        var errorsDictionary = errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray()
            );

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Validation.Failed",
            detail: "Произошла одна или несколько ошибок валидации.",
            extensions: new Dictionary<string, object?>
            {
                { "errors", errorsDictionary }
            }
        );
    }

    private static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError
    };
}