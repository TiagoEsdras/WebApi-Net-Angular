using System;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using AutoMapper;

namespace ProEventos.Application
{
    public class LoteService : ILoteService
    {
        private readonly IGeralPersist _geralPersist;
        private readonly ILotePersist _lotePersist;
        private readonly IMapper _mapper;
        public LoteService(
            IGeralPersist geralPersist,
            ILotePersist lotePersist,
            IMapper mapper)
        {
            _lotePersist = lotePersist;
            _geralPersist = geralPersist;
            _mapper = mapper;

        }

        private async Task AddLote(int eventoId, LoteDto model)
        {
            try
            {
                var lote = _mapper.Map<Lote>(model);
                lote.EventoId = eventoId;
                _geralPersist.Add<Lote>(lote);
                await _geralPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        private async Task UpdateLote(Lote lote, LoteDto model, int eventoId)
        {
            try
            {
                model.EventoId = eventoId;
                _mapper.Map(model, lote);
                _geralPersist.Updade<Lote>(lote);
                await _geralPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoteDto[]> SaveLotes(int eventoId, LoteDto[] models) {
            try
            {
                var lotes = await _lotePersist.GetLotesByEventoId(eventoId);
                if (lotes == null) return null;
                
                foreach (var model in models) {
                    if(model.Id == 0) {
                       await AddLote(eventoId, model);
                    }
                    else {
                        var lote = lotes.FirstOrDefault(lote => lote.Id == model.Id);
                        await UpdateLote(lote, model, eventoId);
                    }                    
                }

                var lotesRetorno = await _lotePersist.GetLotesByEventoId(eventoId);
                return _mapper.Map<LoteDto[]>(lotesRetorno);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteLote(int eventoId, int loteId) {
            try
            {
                var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
                if(lote == null) throw new Exception("Lote para delete não foi encontrado!");

                _geralPersist.Delete<Lote>(lote);

                return await _geralPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoteDto[]> GetLotesByEventoId(int eventoId) {
            try
            {
                var lotes = await _lotePersist.GetLotesByEventoId(eventoId);
                if(lotes == null) throw new Exception("Não foi encontrado nenhum evento!");

                return _mapper.Map<LoteDto[]>(lotes);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoteDto> GetLoteByIdsAsync(int eventoId, int loteId) {
            try
            {
                var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
                if(lote == null) throw new Exception("Não foi encontrado nenhum evento!");

                return _mapper.Map<LoteDto>(lote);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }
        
    }
}