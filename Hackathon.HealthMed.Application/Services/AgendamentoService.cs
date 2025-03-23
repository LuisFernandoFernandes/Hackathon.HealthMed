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
using StackExchange.Redis;
using System.Security.Claims;

namespace Hackathon.HealthMed.Application.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IHorarioRepository _horarioRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDatabase _redisDb;

    public AgendamentoService(
        IAgendamentoRepository agendamentoRepository,
        IMedicoRepository medicoRepository,
        IPacienteRepository pacienteRepository,
        IHorarioRepository horarioRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IConnectionMultiplexer redis)
    {
        _agendamentoRepository = agendamentoRepository;
        _medicoRepository = medicoRepository;
        _pacienteRepository = pacienteRepository;
        _horarioRepository = horarioRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _redisDb = redis.GetDatabase();
    }

    public async Task<ServiceResult<Guid>> AgendarConsulta(AgendarConsultaDTO dto)
    {
        try
        {
            var pacienteId = await ObterPacienteId();

            var horario = await _horarioRepository.BuscarHorarioPorIdEStatus(dto.HorarioId, eStatusHorario.Disponivel);

            if (horario == null)
            {
                throw new ValidacaoException("Horário inválido ou já reservado.");
            }

            var agendamento = new Agendamento(pacienteId, dto.HorarioId);

            await _agendamentoRepository.Adicionar(agendamento);

            horario.AtualizarStatus(eStatusHorario.Reservado);

            await _horarioRepository.Editar(horario);
            var cacheKey = $"horarios_{horario.MedicoId}";
            await _redisDb.KeyDeleteAsync(cacheKey);

            return new ServiceResult<Guid>(agendamento.Id);
        }
        catch (Exception ex)
        {
            return new ServiceResult<Guid>(ex);
        }
    }

    public async Task<ServiceResult<bool>> ConfirmarAgendamento(ConfirmarAgendamentoDTO dto)
    {
        try
        {
            var medicoId = await ObterMedicoId();
            var agendamento = await _agendamentoRepository.BuscarPorId(dto.AgendamentoId);

            if (agendamento == null)
                throw new ValidacaoException("Agendamento não encontrado.");

            var horario = await _horarioRepository.BuscarHorarioPorIdEStatus(agendamento.HorarioId, eStatusHorario.Reservado);
            if (horario == null || horario.MedicoId != medicoId)
                throw new ValidacaoException("Ação não permitida. O horário não pertence ao médico.");

            if (dto.Aceitar)
            {
                agendamento.AtualizarStatus(eStatusAgendamento.Agendado);
            }
            else
            {
                agendamento.AtualizarStatus(eStatusAgendamento.Cancelado, dto.Justificativa);

                horario.AtualizarStatus(eStatusHorario.Disponivel);
                await _horarioRepository.Editar(horario);

                var cacheKey = $"horarios_{horario.MedicoId}";
                await _redisDb.KeyDeleteAsync(cacheKey);
            }

            await _agendamentoRepository.Editar(agendamento);
            return new ServiceResult<bool>(true);
        }
        catch (Exception ex)
        {
            return new ServiceResult<bool>(ex);
        }
    }

    public async Task<ServiceResult<bool>> CancelarPorMedico(CancelarAgendamentoMedicoDTO dto)
    {
        try
        {
            var medicoId = await ObterMedicoId();

            var agendamento = await _agendamentoRepository.BuscarPorId(dto.AgendamentoId);
            if (agendamento == null)
                throw new ValidacaoException("Agendamento não encontrado.");

            var horario = await _horarioRepository.BuscarHorarioPorIdEStatus(agendamento.HorarioId, eStatusHorario.Reservado);
            if (horario == null || horario.MedicoId != medicoId)
                throw new ValidacaoException("Ação não permitida. O horário não está disponível para o médico.");

            agendamento.AtualizarStatus(eStatusAgendamento.Cancelado, dto.Justificativa);

            horario.AtualizarStatus(eStatusHorario.Disponivel);
            await _horarioRepository.Editar(horario);
            var cacheKey = $"horarios_{medicoId}";
            await _redisDb.KeyDeleteAsync(cacheKey);

            await _agendamentoRepository.Editar(agendamento);

            return new ServiceResult<bool>(true);
        }
        catch (Exception ex)
        {
            return new ServiceResult<bool>(ex);
        }
    }

    public async Task<ServiceResult<bool>> CancelarPorPaciente(CancelarAgendamentoPacienteDTO dto)
    {
        try
        {
            var pacienteId = await ObterPacienteId();

            var agendamento = await _agendamentoRepository.BuscarPorId(dto.AgendamentoId);
            if (agendamento == null || (agendamento.Status != eStatusAgendamento.Agendado && agendamento.Status != eStatusAgendamento.Pendente))
                throw new ValidacaoException("Agendamento não encontrado.");

            if (agendamento.PacienteId != pacienteId)
                throw new ValidacaoException("Agendamento não pertence ao paciente.");

            if (string.IsNullOrWhiteSpace(dto.Justificativa))
                throw new ValidacaoException("Justificativa é obrigatória para cancelamento.");

            agendamento.AtualizarStatus(eStatusAgendamento.CanceladoPaciente, dto.Justificativa);

            var horario = await _horarioRepository.BuscarHorarioPorIdEStatus(agendamento.HorarioId, eStatusHorario.Reservado);
            if (horario != null)
            {
                horario.AtualizarStatus(eStatusHorario.Disponivel);
                await _horarioRepository.Editar(horario);
                var cacheKey = $"horarios_{horario.MedicoId}";
                await _redisDb.KeyDeleteAsync(cacheKey);
            }

            await _agendamentoRepository.Editar(agendamento);

            return new ServiceResult<bool>(true);
        }
        catch (Exception ex)
        {
            return new ServiceResult<bool>(ex);
        }
    }

    private Guid ObterUsuarioId()
    {
        var usuarioIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(usuarioIdClaim))
        {
            throw new ValidacaoException("Usuário não autenticado.");
        }
        return Guid.Parse(usuarioIdClaim);
    }

    private async Task<Guid> ObterPacienteId()
    {
        var usuarioId = ObterUsuarioId();
        var pacienteId = await _pacienteRepository.BuscarPacientePorUsuarioId(usuarioId);
        if (pacienteId == Guid.Empty)
        {
            throw new ValidacaoException("Paciente não encontrado.");
        }
        return pacienteId;
    }

    private async Task<Guid> ObterMedicoId()
    {
        var usuarioId = ObterUsuarioId();
        var medicoId = await _medicoRepository.BuscarMedicoPorUsuarioId(usuarioId);
        if (medicoId == Guid.Empty)
        {
            throw new ValidacaoException("Médico não encontrado.");
        }
        return medicoId;
    }

}
