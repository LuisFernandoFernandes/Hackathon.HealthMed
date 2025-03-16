using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Domain.Entities;

public class HorarioMappingProfile : Profile
{
    public HorarioMappingProfile()
    {
        CreateMap<Horario, HorarioDTO>();

        CreateMap<CadastrarHorarioDTO, Horario>()
            .ForMember(dest => dest.MedicoId, opt => opt.MapFrom(src => src.MedicoId ?? Guid.Empty));

        CreateMap<EditarHorarioDTO, Horario>()
            .ForMember(dest => dest.MedicoId, opt => opt.Ignore())
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null)
            );
    }
}
