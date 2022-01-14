using AutoMapper;
using ProEventos.API.Dtos;
using ProEventos.Domain;

namespace ProEventos.API.Helpers
{
    public class ProEventosProfile : Profile
    {
        public ProEventosProfile() {
            CreateMap<Evento, EventoDto>();
        }
    }
}