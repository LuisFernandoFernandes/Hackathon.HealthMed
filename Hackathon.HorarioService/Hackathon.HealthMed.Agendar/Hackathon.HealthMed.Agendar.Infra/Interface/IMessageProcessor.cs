using Hackathon.HealthMed.Agendar.Infra.Model;

namespace Hackathon.HealthMed.Agendar.Infra.Interface;

public interface IMessageProcessor
{
    Task<ServiceResult<bool>> ProcessMessageAsync(string message);
}
