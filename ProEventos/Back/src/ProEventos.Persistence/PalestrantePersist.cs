using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProEventos.Persistence
{
    public class PalestrantePersist : GeralPersist, IPalestrantePersist
    {
        private readonly ProEventosContext _context;

        public PalestrantePersist(ProEventosContext context) : base(context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<PageList<Palestrante>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .AsNoTracking()
                .Include(palestrante => palestrante.User)
                .Include(palestrante => palestrante.RedesSociais)
                .Where(palestrante => palestrante.User.Funcao == Domain.Enum.Funcao.Palestrante);

            if (includeEventos)
                query = query
                    .Include(evento => evento.PalestrantesEventos)
                    .ThenInclude(palestranteEvento => palestranteEvento.Evento);

            if (pageParams.Term is not null)
                query = query.Where(p => p.MiniCurriculo.ToLower().Contains(pageParams.Term.ToLower()) ||
                                          p.User.PrimeiroNome.ToLower().Contains(pageParams.Term.ToLower()) ||
                                          p.User.UltimoNome.ToLower().Contains(pageParams.Term.ToLower()));

            query = query.OrderBy(palestrante => palestrante.Id);

            return await PageList<Palestrante>.CreateAsync(query, pageParams.PageNumber, pageParams.PageSize);
        }

        public async Task<Palestrante> GetPalestranteByIdAsync(int userId, bool includeEventos)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .AsNoTracking()
                .Include(palestrante => palestrante.User)
                .Include(palestrante => palestrante.RedesSociais);

            if (includeEventos)
                query = query
                .Include(evento => evento.PalestrantesEventos)
                .ThenInclude(palestranteEvento => palestranteEvento.Evento);

            query = query
                .OrderBy(palestrante => palestrante.Id)
                .Where(palestrante => palestrante.UserId == userId);

            return await query.FirstOrDefaultAsync();
        }
    }
}