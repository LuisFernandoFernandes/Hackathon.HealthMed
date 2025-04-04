namespace Hackathon.HealthMed.Agendar.Infra.Model;

public class ServiceResult<T>
{
    public T? Data { get; }
    public Exception? Error { get; }
    public bool IsSuccess => Error == null;
    public ServiceResult(T data)
    {
        Data = data;
        Error = null;
    }

    public ServiceResult(Exception error)
    {
        Data = default;
        Error = error;
    }
}
