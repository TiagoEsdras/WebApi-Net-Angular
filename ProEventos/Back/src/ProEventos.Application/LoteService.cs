using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        private async Task AddLote(int eventoId, LoteDto loteDto)
        {
            var lote = _mapper.Map<Lote>(loteDto);
            lote.EventoId = eventoId;
            _geralPersist.Add<Lote>(lote);
            await _geralPersist.SaveChangesAsync();
        }

        private async Task UpdateLote(Lote lote, LoteDto loteDto, int eventoId)
        {
            loteDto.EventoId = eventoId;
            _mapper.Map(loteDto, lote);
            _geralPersist.Update<Lote>(lote);
            await _geralPersist.SaveChangesAsync();
        }

        public async Task<LoteDto[]> SaveLotes(int eventoId, LoteDto[] loteDtos)
        {
            var lotes = await _lotePersist.GetLotesByEventoId(eventoId);
            if (lotes == null) return null;

            foreach (var model in loteDtos)
            {
                if (model.Id == 0)
                {
                    await AddLote(eventoId, model);
                }
                else
                {
                    var lote = lotes.FirstOrDefault(lote => lote.Id == model.Id);
                    await UpdateLote(lote, model, eventoId);
                }
            }

            var lotesRetorno = await _lotePersist.GetLotesByEventoId(eventoId);
            return _mapper.Map<LoteDto[]>(lotesRetorno);
        }

        public async Task<bool> DeleteLote(int eventoId, int loteId)
        {
            var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
            if (lote == null) throw
                new Exception("Lote para delete não foi encontrado!");

            _geralPersist.Delete<Lote>(lote);

            return await _geralPersist.SaveChangesAsync();
        }

        public async Task<LoteDto[]> GetLotesByEventoId(int eventoId)
        {
            var lotes = await _lotePersist.GetLotesByEventoId(eventoId);
            if (lotes == null)
                throw new Exception("Não foi encontrado nenhum evento!");

            return _mapper.Map<LoteDto[]>(lotes);
        }

        public async Task<LoteDto> GetLoteByIdsAsync(int eventoId, int loteId)
        {
            var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
            if (lote == null)
                throw new Exception("Não foi encontrado nenhum evento!");

            return _mapper.Map<LoteDto>(lote);
        }
    }
}