using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class LotePersist : ILotePersist
    {
        private readonly ProEventosContext _context;
        public LotePersist(ProEventosContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        }  

        public async Task<Lote[]> GetLotesByEventoId(int eventoId)
        {
            IQueryable<Lote> query = _context.Lotes
                .AsNoTracking()                
                .Where(lote => lote.EventoId == eventoId);

            query = query.OrderBy(lote => lote.Id);

            return await query.ToArrayAsync();
        }
       
        public async Task<Lote> GetLoteByIdsAsync(int eventoId, int loteId)
        {
            IQueryable<Lote> query = _context.Lotes
                .AsNoTracking()                
                .Where(lote => lote.EventoId == eventoId  && lote.Id == loteId);

            return await query.FirstOrDefaultAsync();
        }
    }
}