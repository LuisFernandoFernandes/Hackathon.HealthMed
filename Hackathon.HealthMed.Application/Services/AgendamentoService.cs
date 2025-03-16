using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Model;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Interfaces;
using Hackathon.HealthMed.Infra.Repository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Hackathon.HealthMed.Application.Services;

public class AgendamentoService(IAgendamentoRepository _agendamentoRepository, IPacienteRepository _pacienteRepository, IHorarioRepository _horarioRepository, IMapper _mapper, IHttpContextAccessor _httpContextAccessor) : IAgendamentoService
{
    public async Task<ServiceResult<Guid>> AgendarConsulta(AgendarConsultaDTO dto)
    {
        try
        {
            var pacienteId = await ObterPacienteId();

            var horario = await _horarioRepository.BuscarHorarioDisponivel(dto.HorarioId);

            if (horario == null)
            {
                throw new ValidacaoException("Horário inválido ou já reservado.");
            }

            var agendamento = new Agendamento(pacienteId, dto.HorarioId);

            await _agendamentoRepository.Adicionar(agendamento);

            horario.AtualizarStatus(eStatusHorario.Reservado);

            await _horarioRepository.Editar(horario);

            return new ServiceResult<Guid>(agendamento.Id);
        }
        catch (Exception ex)
        {
            return new ServiceResult<Guid>(ex);
        }
    }

    private async Task<Guid> ObterPacienteId()
    {
        var usuarioIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdClaim))
        {
            throw new ValidacaoException("Usuário não autenticado.");
        }

        var pacienteId = await _pacienteRepository.BuscarPacientePorUsuarioId(Guid.Parse(usuarioIdClaim));

        if (pacienteId == Guid.Empty)
        {
            throw new ValidacaoException("Paciente não encontrado.");
        }

        return pacienteId;
    }

}
