using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Hackathon.HealthMed.Domain.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum eStatusHorario
{
    Disponivel = 0,
    Reservado = 1,
    Cancelado = 2
}
