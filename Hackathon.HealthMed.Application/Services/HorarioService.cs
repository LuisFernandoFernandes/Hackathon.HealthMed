using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Model;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Hackathon.HealthMed.Application.Services;

public class HorarioService(IHorarioRepository _horarioRepository, IMedicoRepository _medicoRepository, IMapper _mapper, IHttpContextAccessor _httpContextAccessor) : IHorarioService
{
    public async Task<ServiceResult<Guid>> CadastrarHorario(CadastrarHorarioDTO horarioDto)
    {
        try
        {
            var medicoIdAutenticado = await ObterMedicoId();

            if (horarioDto.MedicoId.HasValue && horarioDto.MedicoId.Value != medicoIdAutenticado)
            {
                throw new ValidacaoException("Você não tem permissão para cadastrar horários para outro médico.");
            }

            var horarioExistente = await _horarioRepository.BuscarHorarioPorMedicoEData(medicoIdAutenticado, horarioDto.DataHorario);
            if (horarioExistente)
            {
                throw new ValidacaoException("Horário já cadastrado para essa data/hora.");
            }

            horarioDto.MedicoId = medicoIdAutenticado;

            var horario = _mapper.Map<Horario>(horarioDto);

            await _horarioRepository.Cadastrar(horario);

            return new ServiceResult<Guid>(horario.Id);
        }
        catch (Exception ex)
        {
            return new ServiceResult<Guid>(ex);
        }
    }


    public async Task<ServiceResult<bool>> EditarHorario(EditarHorarioDTO horarioDto)
    {
        try
        {
            var medicoId = await ObterMedicoId();

            var horarios = await _horarioRepository.BuscarHorarioParaEdicao(medicoId, horarioDto.Id, horarioDto.DataHorario);

            var horario = horarios.FirstOrDefault(h => h.Id == horarioDto.Id);
            var horarioDuplicado = horarios.FirstOrDefault(h => h.Id != horarioDto.Id);

            if (horario == null)
            {
                throw new ValidacaoException("Horário não encontrado ou não pertence ao médico logado.");
            }

            if (horarioDuplicado != null)
            {
                throw new ValidacaoException("Já existe um horário cadastrado para essa data/hora.");
            }

            _mapper.Map(horarioDto, horario);

            await _horarioRepository.Editar(horario);

            return new ServiceResult<bool>(true);
        }
        catch (Exception ex)
        {
            return new ServiceResult<bool>(ex);
        }
    }

    public async Task<ServiceResult<IEnumerable<HorarioDTO>>> BuscarHorarios()
    {
        try
        {
            var medicoId = await ObterMedicoId();
            var horarios = await _horarioRepository.BuscarHorarios(medicoId);

            var horariosDto = _mapper.Map<IEnumerable<HorarioDTO>>(horarios);

            return new ServiceResult<IEnumerable<HorarioDTO>>(horariosDto);
        }
        catch (Exception ex)
        {
            return new ServiceResult<IEnumerable<HorarioDTO>>(ex);
        }
    }



    private async Task<Guid> ObterMedicoId()
    {
        var usuarioIdClaid = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdClaid))
        {
            throw new ValidacaoException("Usuário não autenticado");
        }

        var medicoId = await _medicoRepository.BuscarMedicoPorUsuarioId(Guid.Parse(usuarioIdClaid));

        if (medicoId == Guid.Empty)
        {
            throw new ValidacaoException("Médico não encontrado");
        }

        return medicoId;
    }
}
