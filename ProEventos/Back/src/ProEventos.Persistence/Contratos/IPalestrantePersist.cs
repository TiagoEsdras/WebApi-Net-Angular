using ProEventos.Domain;
using ProEventos.Persistence.Models;
using System.Threading.Tasks;

namespace ProEventos.Persistence.Contratos
{
    public interface IPalestrantePersist : IGeralPersist
    {
        //Palestrantes

        Task<PageList<Palestrante>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false);

        Task<Palestrante> GetPalestranteByIdAsync(int userId, bool includeEventos = false);
    }
}