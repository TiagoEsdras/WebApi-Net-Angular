using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;
using System.Linq;
using System.Threading.Tasks;

namespace ProEventos.Persistence
{
    public class RedeSocialPersist : GeralPersist, IRedeSocialPersist
    {
        private readonly ProEventosContext _context;

        public RedeSocialPersist(ProEventosContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RedeSocial> GetRedeSocialEventoByIdsAsync(int eventoId, int id)
        {
            IQueryable<RedeSocial> query = _context.RedesSociais
                .AsNoTracking()
                .Where(redeSocial => redeSocial.EventoId == eventoId && redeSocial.Id == id);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<RedeSocial> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int id)
        {
            IQueryable<RedeSocial> query = _context.RedesSociais
                .AsNoTracking()
                .Where(redeSocial => redeSocial.PalestranteId == palestranteId && redeSocial.Id == id);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<RedeSocial[]> GetAllByEventoIdAsync(int eventoId)
        {
            IQueryable<RedeSocial> query = _context.RedesSociais
                .AsNoTracking()
                .Where(redeSocial => redeSocial.EventoId == eventoId);

            return await query.ToArrayAsync();
        }

        public async Task<RedeSocial[]> GetAllByPalestranteIdAsync(int palestranteId)
        {
            IQueryable<RedeSocial> query = _context.RedesSociais
                .AsNoTracking()
                .Where(redeSocial => redeSocial.PalestranteId == palestranteId);

            return await query.ToArrayAsync();
        }
    }
}