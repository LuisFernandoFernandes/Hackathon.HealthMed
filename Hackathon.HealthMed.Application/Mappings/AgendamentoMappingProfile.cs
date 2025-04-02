using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon.HealthMed.Application.Mappings;

public class AgendamentoMappingProfile : Profile
{
    public AgendamentoMappingProfile()
    {
        CreateMap<Agendamento, AgendamentoDTO>();
    }
}
