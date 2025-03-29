using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hackathon.HealthMed.Domain.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum eStatusHorario
{
    Disponivel = 0,
    Reservado = 1,
    Cancelado = 2
}
