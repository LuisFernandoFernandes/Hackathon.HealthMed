using Hackathon.HealthMed.Api.Extension;
using Hackathon.HealthMed.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Hackathon.HealthMed.Api.Filter;

public class ModelStateValidatorFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {

        if (context.Result is BadRequestObjectResult { Value: ValidacaoException } result)
        {
            context.Result = new BadRequestObjectResult(((ValidacaoException)(result.Value)).Message.ConverteParaErro());
        }
        if (context.Result is BadRequestObjectResult { Value: Exception })
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var erros = context.ModelState.RetornaErrosMessages();
            context.Result = new BadRequestObjectResult(erros);
        }
    }
}