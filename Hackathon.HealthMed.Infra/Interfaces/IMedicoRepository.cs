﻿

using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IMedicoRepository
{
    Task<IEnumerable<Medico>> BuscarMedicos(eEspecialidade? especialidade);

    Task<Guid> BuscarMedicoPorUsuarioId(Guid usuarioId);
}
