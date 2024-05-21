using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace InterviewTest.Api.Util
{
    public record ErrorResult(
        Dictionary<string, List<string>> Errors,
        string? Title = null,
        string? Detail = null,
        string? Instance = null,
        int Status = 400);

    public static class ResultsExtensions
    {
        public static IActionResult FluentValidationProblem(
            this ValidationResult validationResult,
            int status = 400,
            string? title = null,
            string? detail = null,
            string? instance = null)
        {
            var errors = new Dictionary<string, List<string>>();
            foreach (var error in validationResult.Errors)
            {
                if (errors.ContainsKey(error.PropertyName))
                {
                    errors[error.PropertyName].Add(error.ErrorMessage);
                }
                else
                {
                    errors[error.PropertyName] = new() { error.ErrorMessage };
                }
            }
            return new BadRequestObjectResult(new ErrorResult(
                    Errors: errors,
                    Title: title,
                    Detail: detail,
                    Instance: instance,
                    Status: status
                ));
        }

        public static IActionResult FluentValidationProblem(
            this ValidationResult validationResult,
            Dictionary<string, List<string>> errors,
            int status = 400,
            string? title = null,
            string? detail = null,
            string? instance = null
            )
        {

            return new ObjectResult(new ErrorResult(
                    Errors: errors,
                    Title: title,
                    Detail: detail,
                    Instance: instance,
                    Status: status
                ))
            {
                StatusCode = status
            };
        }
    }
}
