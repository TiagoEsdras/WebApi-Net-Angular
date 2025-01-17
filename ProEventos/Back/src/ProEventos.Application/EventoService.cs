using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;
using System;
using System.Threading.Tasks;

namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IGeralPersist _geralPersist;
        private readonly IEventoPersist _eventoPersist;
        private readonly IMapper _mapper;

        public EventoService(
            IGeralPersist geralPersist,
            IEventoPersist eventoPersist,
            IMapper mapper)
        {
            _eventoPersist = eventoPersist;
            _geralPersist = geralPersist;
            _mapper = mapper;
        }

        public async Task<EventoDto> AddEventos(int userId, EventoDto eventoDto)
        {
            var evento = _mapper.Map<Evento>(eventoDto);
            _geralPersist.Add<Evento>(evento);

            evento.UserId = userId;

            if (await _geralPersist.SaveChangesAsync())
            {
                var resut = await _eventoPersist.GetEventoByIdAsync(userId, evento.Id, false);
                return _mapper.Map<EventoDto>(resut);
            }
            return null;
        }

        public async Task<EventoDto> UpdateEvento(int userId, int eventoId, EventoDto eventoDto)
        {
            var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
            if (evento == null) return null;

            eventoDto.Id = evento.Id;
            eventoDto.UserId = userId;

            _mapper.Map(eventoDto, evento);

            _geralPersist.Update<Evento>(evento);

            if (await _geralPersist.SaveChangesAsync())
            {
                var result = await _eventoPersist.GetEventoByIdAsync(userId, eventoDto.Id, false);
                return _mapper.Map<EventoDto>(result);
            }
            return null;
        }

        public async Task<bool> DeleteEvento(int userId, int eventoId)
        {
            var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
            if (evento == null)
                throw new Exception("Evento para delete não foi encontrado!");

            _geralPersist.Delete<Evento>(evento);

            return await _geralPersist.SaveChangesAsync();
        }

        public async Task<PageList<EventoDto>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
        {
            var eventos = await _eventoPersist.GetAllEventosAsync(userId, pageParams, includePalestrantes);
            if (eventos == null) return null;

            var resultado = _mapper.Map<PageList<EventoDto>>(eventos);

            resultado.CurrentPage = eventos.CurrentPage;
            resultado.TotalPages = eventos.TotalPages;
            resultado.PageSize = eventos.PageSize;
            resultado.TotalCount = eventos.TotalCount;

            return resultado;
        }

        public async Task<EventoDto> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, includePalestrantes);
            if (evento == null) return null;

            return _mapper.Map<EventoDto>(evento);
        }
    }
}