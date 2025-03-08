using Hackathon.HealthMed.Api.Model;

namespace Hackathon.HealthMed.Api.Extension;

public static class ErrorExtension
{
    public static MensagemErro ConverteParaErro(this string? mensagem)
    {
        return new MensagemErro(new List<string>(new string?[] { mensagem }!));
    }

}
