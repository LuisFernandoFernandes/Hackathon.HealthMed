using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Domain.Entities;

namespace Hackathon.HealthMed.Application.Mappings;

public class UsuarioMappingProfile : Profile
{
    public UsuarioMappingProfile()
    {
        // Entidade -> DTO (Saída)
        //CreateMap<Usuario, UsuarioDTO>()
        //   .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
        //   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        //   .ForMember(dest => dest.TipoUsuario, opt => opt.MapFrom(src => src.TipoUsuario))
        //   .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo))
        //   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        //   .ForMember(dest => dest.Senha, opt => opt.Ignore()); // Não expor senha

        // DTO -> Entidade (Entrada)
        //CreateMap<UsuarioDTO, Usuario>()
        //    .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
        //    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        //    .ForMember(dest => dest.TipoUsuario, opt => opt.MapFrom(src => src.TipoUsuario))
        //    .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo))
        //    .ForMember(dest => dest.SenhaHash, opt => opt.MapFrom(src => HashHelper.HashSenha(src.Senha))) // Gerando hash da senha
        //    .ForMember(dest => dest.Id, opt => opt.Ignore()) // O Id será gerado automaticamente
        //    .ForMember(dest => dest.DataCriacao, opt => opt.Ignore()); // Gerenciado pela entidade
    }
}
