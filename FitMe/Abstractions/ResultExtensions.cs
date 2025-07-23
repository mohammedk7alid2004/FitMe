using Microsoft.AspNetCore.Mvc;

namespace FitMe.Abstractions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {

        if (result.IsSuccess)
            throw new InvalidOperationException("can't convert result success to a problem ");
        var problem_link = Results.Problem(statusCode: result.Error.StatusCode);
        var problemDetails = problem_link.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem_link) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                "errors", new[]{
                  result.Error.code,
                    result.Error.Description

                }
            }
        };

        return new ObjectResult(problemDetails);

    }
}
