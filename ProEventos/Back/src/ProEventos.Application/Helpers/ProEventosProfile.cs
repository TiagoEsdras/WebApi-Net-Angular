using AutoMapper;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Domain.Identity;

namespace ProEventos.Application.Helpers
{
    public class ProEventosProfile : Profile
    {
        public ProEventosProfile() {
            CreateMap<Evento, EventoDto>()
                .ForMember(dto => dto.DataEvento, opt => opt.MapFrom(model => model.DataEvento.GetValueOrDefault().ToString("yyyy-MM-ddTHH:mm:ss")))
                .ReverseMap();
            CreateMap<Lote, LoteDto>()
                .ForMember(dto => dto.DataInicio, opt => opt.MapFrom(model => model.DataInicio.GetValueOrDefault().ToString("yyyy-MM-ddTHH:mm:ss")))
                .ForMember(dto => dto.DataFim, opt => opt.MapFrom(model => model.DataFim.GetValueOrDefault().ToString("yyyy-MM-ddTHH:mm:ss")))
                .ReverseMap();
            CreateMap<Palestrante, PalestranteDto>().ReverseMap();
            CreateMap<RedeSocial, RedeSocialDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
        }
    }
}