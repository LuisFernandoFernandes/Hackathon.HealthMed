using Hackathon.HealthMed.Api.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hackathon.HealthMed.Api.Extension;

public static class ModelStateExtension
{
    public static MensagemErro RetornaErrosMessages(this ModelStateDictionary modelstate)
    {
        return new MensagemErro(modelstate
            .SelectMany(ms => ms.Value!.Errors)
            .Select(e => e.ErrorMessage)
            .ToList());
    }
}
