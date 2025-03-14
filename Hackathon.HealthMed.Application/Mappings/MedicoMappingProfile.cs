using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Domain.Entities;

namespace Hackathon.HealthMed.Application.Mappings;

public class MedicoMappingProfile : Profile
{
    public MedicoMappingProfile()
    {
        CreateMap<Medico, MedicoDTO>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Usuario.Nome))
                 .ForMember(dest => dest.CRM, opt => opt.MapFrom(src => src.CRM))
                 .ForMember(dest => dest.Especialidade, opt => opt.MapFrom(src => src.Especialidade));
    }
}
