using System;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using AutoMapper;

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
        public async Task<EventoDto> AddEventos(int userId, EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);
                _geralPersist.Add<Evento>(evento);

                evento.UserId = userId;

                if(await _geralPersist.SaveChangesAsync()) {
                    var resut = await _eventoPersist.GetEventoByIdAsync(userId, evento.Id, false);
                    return _mapper.Map<EventoDto>(resut);
                }
                return null;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDto> UpdateEvento(int userId, int eventoId, EventoDto model)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
                if(evento == null) return null;

                model.Id = evento.Id;
                model.UserId = userId;
                
                _mapper.Map(model, evento);

                _geralPersist.Updade<Evento>(evento);

                if(await _geralPersist.SaveChangesAsync()) {
                    var result = await _eventoPersist.GetEventoByIdAsync(userId, model.Id, false);
                    return _mapper.Map<EventoDto>(evento);
                }
                return null;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEvento(int userId, int eventoId)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
                if(evento == null) throw new Exception("Evento para delete não foi encontrado!");

                _geralPersist.Delete<Evento>(evento);

                return await _geralPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDto[]> GetAllEventosAsync(int userId, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersist.GetAllEventosAsync(userId, includePalestrantes);
                if(eventos == null) return null;

                return _mapper.Map<EventoDto[]>(eventos);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDto[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersist.GetAllEventosByTemaAsync(userId, tema, includePalestrantes);
                if(eventos == null) return null;
                
                return _mapper.Map<EventoDto[]>(eventos);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDto> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(userId, eventoId, includePalestrantes);
                if(evento == null) return null;

                return _mapper.Map<EventoDto>(evento);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }
    }
}