namespace Hackathon.HealthMed.Api.Model;

public class MensagemErro
{
    public MensagemErro(List<string> erros)
    {
        mensagemErro = erros;
    }
    public List<string> mensagemErro { get; private set; }
}
